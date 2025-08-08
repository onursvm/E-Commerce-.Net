namespace E_Commerce.Business.AuthServices.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendPasswordResetEmailAsync(string email, string resetToken);
    }
}
