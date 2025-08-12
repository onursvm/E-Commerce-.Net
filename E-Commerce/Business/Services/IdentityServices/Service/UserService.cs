using E_Commerce.Business.Services.IdentityServices.Interfaces;
using E_Commerce.DataAccses.Entities.Identity;
using E_Commerce.DataAccses.Interfaces.Identity;
using E_Commerce.Presentation.Dtos.Auth;

namespace E_Commerce.Business.Services.IdentityServices.Service
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (!await _userRepository.CheckPasswordAsync(userId, currentPassword))
                throw new InvalidOperationException("Current password is incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty");

            user.Email = user.Email.Trim();
            user.UserName = user.UserName.Trim();
            if (!string.IsNullOrWhiteSpace(user.FullName))
                user.FullName = user.FullName.Trim();

            // Email kontrolü
            if (await _userRepository.GetByEmailAsync(user.Email) != null)
                throw new InvalidOperationException("Email already in use");

            // Username kontrolü
            if (await _userRepository.GetByUsernameAsync(user.UserName) != null)
                throw new InvalidOperationException("Username already taken");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.IsActive = true;

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            await _userRepository.DeleteAsync(id);
        }

        public  async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email) != null;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user ?? throw new KeyNotFoundException("User not found");
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user ?? throw new KeyNotFoundException("User not found");
        }

        public Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ToggleUserStatusAsync(int id, bool isActive)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.IsActive = isActive;
            await _userRepository.UpdateAsync(user);
            return user.IsActive;
        }

        public async Task UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Email değişiklik kontrolü
            if (user.Email != updateProfileDto.Email &&
                await _userRepository.GetByEmailAsync(updateProfileDto.Email) != null)
            {
                throw new InvalidOperationException("New email already in use");
            }

            // Username değişiklik kontrolü
            if (user.UserName != updateProfileDto.UserName &&
                await _userRepository.GetByUsernameAsync(updateProfileDto.UserName) != null)
            {
                throw new InvalidOperationException("Username already taken");
            }

            user.UserName = updateProfileDto.UserName.Trim();
            user.Email = updateProfileDto.Email.Trim();
            user.FullName = updateProfileDto.FullName?.Trim();

            await _userRepository.UpdateAsync(user);
        }

        public async Task<User> UpdateUserAsync(int id, User updatedUser)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                throw new KeyNotFoundException("User not found");

            updatedUser.Email = updatedUser.Email.Trim();
            updatedUser.UserName = updatedUser.UserName.Trim();
            if (!string.IsNullOrWhiteSpace(updatedUser.FullName))
                updatedUser.FullName = updatedUser.FullName.Trim();

            // Email değişiklik kontrolü
            if (existingUser.Email != updatedUser.Email &&
                await _userRepository.GetByEmailAsync(updatedUser.Email) != null)
            {
                throw new InvalidOperationException("New email already in use");
            }

            // Username değişiklik kontrolü
            if (existingUser.UserName != updatedUser.UserName &&
                await _userRepository.GetByUsernameAsync(updatedUser.UserName) != null)
            {
                throw new InvalidOperationException("Username already taken");
            }

            existingUser.UserName = updatedUser.UserName;
            existingUser.Email = updatedUser.Email;
            existingUser.FullName = updatedUser.FullName;
            existingUser.IsActive = updatedUser.IsActive;

            await _userRepository.UpdateAsync(existingUser);
            return existingUser;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId) != null;
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            return await _userRepository.CheckPasswordAsync(user.Id, password);
        }
    }
}
