using E_Commerce.DataAccses.Entities.Identity;
using E_Commerce.Presentation.Dtos.Auth;

namespace E_Commerce.Business.Services.IdentityServices.Interfaces
{
    public interface IUserService
    {
        // Kullanıcı Yönetimi
        Task<User> CreateUserAsync(User user, string password);
        Task<User> UpdateUserAsync(int id, User updatedUser);
        Task DeleteUserAsync(int id);
        Task<bool> ToggleUserStatusAsync(int id, bool isActive);
        Task UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto);

        // Okuma
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);

        // Kimlik Doğrulama
        Task<bool> ValidateCredentialsAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

        // Yardımcı Metodlar
        Task<bool> UserExistsAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
    }
}
