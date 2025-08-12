using E_Commerce.Business.AuthServices.Interfaces;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Business.AuthServices.Services
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailSection = _configuration.GetSection("EmailSettings");
            var host = emailSection["SmtpServer"];
            var port = emailSection.GetValue<int?>("Port");
            var username = emailSection["Username"];
            var password = emailSection["Password"];
            var fromAddress = emailSection["FromAddress"];
            var displayName = emailSection["DisplayName"];
            var enableSsl = emailSection.GetValue<bool?>("EnableSsl") ?? true;
            var failSilently = emailSection.GetValue<bool?>("FailSilently") ?? true;

            if (string.IsNullOrWhiteSpace(host) || port is null || string.IsNullOrWhiteSpace(fromAddress))
            {
                throw new InvalidOperationException("EmailSettings configuration is missing required values (SmtpServer, Port, FromAddress).");
            }

            using (var client = new SmtpClient(host))
            {
                client.Port = port.Value;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                {
                    client.Credentials = new NetworkCredential(username, password);
                }
                client.EnableSsl = enableSsl;

                var mailMessage = new MailMessage
                {
                    From = string.IsNullOrWhiteSpace(displayName)
                        ? new MailAddress(fromAddress)
                        : new MailAddress(fromAddress, displayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(to);

                try
                {
                    await client.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email to {To}", to);
                    if (!failSilently)
                    {
                        throw;
                    }
                    // else swallow to prevent 500s in endpoints like forgot-password
                }
            }
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            var resetLink = $"{_configuration["ClientApp:Url"]}/reset-password?token={resetToken}";
            var body = $"Şifre sıfırlama linkiniz: <a href='{resetLink}'>Tıklayınız</a>";

            await SendEmailAsync(email, "Şifre Sıfırlama Talebi", body);
        }
    }
}
