namespace MutualFundAPI.Models.DTOs;

public class RegisterDTO
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class ForgotPasswordDTO
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDTO
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ForgotPasswordResponseDTO
{
    public string Message { get; set; } = string.Empty;
    public string? ResetToken { get; set; } // In production, this would be sent via email only
}
