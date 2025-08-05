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
         user.PasswordHash=BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public Task<bool> CheckPasswordAsync(int UserId, string password)
        {
            var user = _context.Users.Find(UserId);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.VerifyAsync(password, user.PasswordHash); 
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
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByIdAsync(int id)
        {
           return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null) return;

            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.IsActive = user.IsActive;
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }
            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
            
        }
    }
}
