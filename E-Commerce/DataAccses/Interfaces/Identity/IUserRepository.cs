using E_Commerce.DataAccses.Entities.Identity;
namespace E_Commerce.DataAccses.Interfaces.Identity
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<bool> CheckPasswordAsync(int UserId, string password);

        Task UpdateRefreshTokenAsync(int userId, string? refreshToken, DateTime? expiryTime);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task CreatePasswordResetTokenAsync(int userId, string token, DateTime expires);
        Task<User?> GetByPasswordResetTokenAsync(string token);
        Task ResetPasswordAsync(int userId, string newPassword);
    }
}
