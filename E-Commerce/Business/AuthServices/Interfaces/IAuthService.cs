using E_Commerce.DataAccses.Entities.Identity;
using System.Security.Claims;

namespace E_Commerce.Business.AuthServices.Interfaces
{
    public interface IAuthService
    {
        // Authentication
        Task<User> LoginAsync(string email, string password);
        Task<User> RefreshTokenAsync(string token, string refreshToken);
        Task<bool> RevokeTokenAsync(string token);

        //Token Operations
        Task<string> GenerateJwtToken(User user);
        ClaimsPrincipal? ValidateJwtToken(string token);
        Task<string> GenerateRefreshTokenAsync(int userId);
        Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken);

        // Password Operations
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);

        // Email Operations
        Task SendEmailAsync(string to, string subject, string body);
        Task SendPasswordResetEmailAsync(string email, string resetToken);



    }
}
