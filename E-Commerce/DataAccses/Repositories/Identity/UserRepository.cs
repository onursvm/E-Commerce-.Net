using E_Commerce.DataAccses.Context;
using E_Commerce.DataAccses.Entities.Identity;
using E_Commerce.DataAccses.Interfaces.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccses.Repositories.Identity
{
    public class UserRepository : IUserRepository
    {
        private readonly CommerceDbContext _context;
        
        public UserRepository(CommerceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckPasswordAsync(int userId, string password)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash); 
        }

        public async Task CreatePasswordResetTokenAsync(int userId, string token, DateTime expires)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.PasswordResetToken = token;
                user.ResetTokenExpires = expires;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var user =await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            email = email.Trim().ToLower();
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email);
        }

        public async Task<User> GetByIdAsync(int id)
        {
           return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetByPasswordResetTokenAsync(string token)
        {
            return await _context.Users
                    .FirstOrDefaultAsync(u => u.PasswordResetToken == token &&
                                           u.ResetTokenExpires > DateTime.UtcNow);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            username = username.Trim().ToLower();
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == username);
        }

        public async Task ResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.PasswordResetToken = null;
                user.ResetTokenExpires = null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null) return;

            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.FullName = user.FullName;
            existingUser.IsActive = user.IsActive;
            
            // Sadece PasswordHash değişmişse hash'le
            if (!string.IsNullOrEmpty(user.PasswordHash) && user.PasswordHash != existingUser.PasswordHash)
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }
            
            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokenAsync(int userId, string? refreshToken, DateTime? expiryTime)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = expiryTime;
                await _context.SaveChangesAsync();
            }
        }
    }
}
