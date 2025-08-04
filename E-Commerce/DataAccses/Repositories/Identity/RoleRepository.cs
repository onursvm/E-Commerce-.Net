using E_Commerce.DataAccses.Context;
using E_Commerce.DataAccses.Entities.Identity;
using E_Commerce.DataAccses.Interfaces.Identity;

namespace E_Commerce.DataAccses.Repositories.Identity
{
    public class RoleRepository : IRoleRepository
    {
        private readonly CommerceDbContext _context;
        public RoleRepository(CommerceDbContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<Role>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Role> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Role> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
