using E_Commerce.Business.AuthServices.Interfaces;
using System.Net.Mail;
using System.Net;

namespace E_Commerce.Business.AuthServices.Services
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            //kullandığımız emaİL servisine göre
            using (var client = new SmtpClient(_configuration["Email:Host"]))
            {
                client.Port = int.Parse(_configuration["Email:Port"]);
                client.Credentials = new NetworkCredential(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:From"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
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
