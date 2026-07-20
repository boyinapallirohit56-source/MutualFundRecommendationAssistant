using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly EmailService _emailService;

    public AuthService(AppDbContext context, IConfiguration config, EmailService emailService)
    {
        _context = context;
        _config = config;
        _emailService = emailService;
    }

    public async Task<AuthResponseDTO?> Register(RegisterDTO dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return null;

        // Generate email verification token
        var verificationToken = Guid.NewGuid().ToString("N");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User",
            IsEmailVerified = false,
            EmailVerificationToken = verificationToken,
            EmailVerificationExpiry = DateTime.UtcNow.AddHours(24)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Send verification email
        await _emailService.SendVerificationEmail(user.Email, user.Name, verificationToken);

        return new AuthResponseDTO
        {
            Token = GenerateToken(user),
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            IsEmailVerified = false,
            Message = "Registration successful. Please check your email to verify your account."
        };
    }

    public async Task<AuthResponseDTO?> Login(LoginDTO dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        if (!user.IsEmailVerified)
        {
            return new AuthResponseDTO
            {
                Token = "",
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsEmailVerified = false,
                Message = "Please verify your email before logging in. Check your inbox."
            };
        }

        return new AuthResponseDTO
        {
            Token = GenerateToken(user),
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            IsEmailVerified = true
        };
    }

    public async Task<bool> VerifyEmail(string token)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.EmailVerificationToken == token &&
            u.EmailVerificationExpiry > DateTime.UtcNow &&
            !u.IsEmailVerified);

        if (user == null) return false;

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationExpiry = null;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResendVerificationEmail(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsEmailVerified);
        if (user == null) return false;

        var newToken = Guid.NewGuid().ToString("N");
        user.EmailVerificationToken = newToken;
        user.EmailVerificationExpiry = DateTime.UtcNow.AddHours(24);
        await _context.SaveChangesAsync();

        await _emailService.SendVerificationEmail(user.Email, user.Name, newToken);
        return true;
    }

    public async Task<ForgotPasswordResponseDTO?> ForgotPassword(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return new ForgotPasswordResponseDTO { Message = "If the email exists, a reset link has been sent." };

        // Generate reset token
        var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

        _context.Set<Models.Entities.PasswordResetToken>().Add(new Models.Entities.PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        });
        await _context.SaveChangesAsync();

        // In production, this token would be sent via email
        // For development, we return it in the response
        return new ForgotPasswordResponseDTO
        {
            Message = "If the email exists, a reset link has been sent.",
            ResetToken = token // Remove this in production — only for dev/testing
        };
    }

    public async Task<bool> ResetPassword(string token, string newPassword)
    {
        var resetToken = await _context.Set<Models.Entities.PasswordResetToken>()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);

        if (resetToken == null) return false;

        resetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        resetToken.IsUsed = true;
        await _context.SaveChangesAsync();

        return true;
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
