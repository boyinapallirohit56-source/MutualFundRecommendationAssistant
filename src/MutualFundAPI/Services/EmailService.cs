using System.Net;
using System.Net.Mail;

namespace MutualFundAPI.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<bool> SendVerificationEmail(string toEmail, string userName, string verificationToken)
    {
        var baseUrl = _config["App:BaseUrl"] ?? "http://localhost:5000";
        var verificationLink = $"{baseUrl}/api/v1/auth/verify-email?token={verificationToken}";

        var subject = "Verify Your Email - Mutual Fund Advisor";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background: #f9fafb; padding: 32px; border-radius: 12px;'>
                    <h2 style='color: #2563eb;'>Welcome to Mutual Fund Advisor!</h2>
                    <p>Hi {userName},</p>
                    <p>Thank you for registering. Please verify your email address by clicking the button below:</p>
                    <div style='text-align: center; margin: 24px 0;'>
                        <a href='{verificationLink}' style='background: #2563eb; color: white; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: bold;'>Verify Email</a>
                    </div>
                    <p style='font-size: 13px; color: #6b7280;'>Or copy this link: {verificationLink}</p>
                    <p style='font-size: 13px; color: #6b7280;'>This link expires in 24 hours.</p>
                    <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 24px 0;'>
                    <p style='font-size: 12px; color: #9ca3af;'>If you didn't create this account, you can ignore this email.</p>
                </div>
            </body>
            </html>";

        return await SendEmail(toEmail, subject, body);
    }

    public async Task<bool> SendPasswordResetEmail(string toEmail, string userName, string resetToken)
    {
        var baseUrl = _config["App:FrontendUrl"] ?? "http://localhost:4200";
        var resetLink = $"{baseUrl}/reset-password?token={resetToken}";

        var subject = "Password Reset - Mutual Fund Advisor";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background: #f9fafb; padding: 32px; border-radius: 12px;'>
                    <h2 style='color: #2563eb;'>Password Reset Request</h2>
                    <p>Hi {userName},</p>
                    <p>We received a request to reset your password. Click the button below:</p>
                    <div style='text-align: center; margin: 24px 0;'>
                        <a href='{resetLink}' style='background: #2563eb; color: white; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: bold;'>Reset Password</a>
                    </div>
                    <p style='font-size: 13px; color: #6b7280;'>This link expires in 1 hour.</p>
                    <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 24px 0;'>
                    <p style='font-size: 12px; color: #9ca3af;'>If you didn't request this, you can ignore this email.</p>
                </div>
            </body>
            </html>";

        return await SendEmail(toEmail, subject, body);
    }

    private async Task<bool> SendEmail(string toEmail, string subject, string htmlBody)
    {
        try
        {
            var smtpHost = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_config["Email:SmtpPort"] ?? "587");
            var senderEmail = _config["Email:SenderEmail"] ?? "";
            var senderPassword = _config["Email:SenderPassword"] ?? "";
            var senderName = _config["Email:SenderName"] ?? "Mutual Fund Advisor";

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
            {
                _logger.LogWarning("Email not configured. Skipping send to {Email}. Subject: {Subject}", toEmail, subject);
                return false;
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            return false;
        }
    }
}
