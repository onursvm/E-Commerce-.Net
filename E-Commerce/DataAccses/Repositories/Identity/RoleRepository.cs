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
    }
}
