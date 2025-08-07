using E_Commerce.DataAccses.Context;
using E_Commerce.DataAccses.Entities.Identity;
using E_Commerce.DataAccses.Interfaces.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccses.Repositories.Identity
{
    public class RoleRepository : IRoleRepository
    {
        private readonly CommerceDbContext _context;
        public RoleRepository(CommerceDbContext context)
        {
            _context = context;
        }

        public async Task<Role> AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task AddRoleToUserAsync(int userId, int roleId)
        {
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Role role)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Role> GetByIdAsync(int id)
        {
           
                return await _context.Roles
                   .AsNoTracking()
                   .FirstOrDefaultAsync(r => r.Id == id);
            

        }

        public async Task<Role> GetByNameAsync(string name)
        {
            return await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                            .Where(ur => ur.UserId == userId)
                            .Select(ur => ur.Role)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersInRoleAsync(int roleId)
        {
            return await _context.UserRoles
                            .Where(ur => ur.RoleId == roleId)
                            .Select(ur => ur.User)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var userRole = await _context.UserRoles
                            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> RoleExistsAsync(int roleId)
        {
            return await _context.Roles
                            .AnyAsync(r => r.Id == roleId);
        }

        public async Task<bool> RoleNameExistsAsync(string name)
        {
            return await _context.Roles
                            .AnyAsync(r => r.Name == name);
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserHasRoleAsync(int userId, int roleId)
        {
            return await _context.UserRoles
                            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }
    }
}
