using E_Commerce.DataAccses.Context;
using E_Commerce.DataAccses.Entities.Properties;
using E_Commerce.DataAccses.Interfaces.Properties;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccses.Repositories.Properties
{
    public class PropertyStatusRepository : IPropertyStatusRepository
    {
        private readonly CommerceDbContext _context;
        public PropertyStatusRepository(CommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PropertyStatus>> GetAllAsync()
        {
            return await _context.PropertyStatus
                .Include(ps => ps.Properties)
                .ToListAsync();
        }

        public async Task<PropertyStatus> GetByIdAsync(int id)
        {
            return await _context.PropertyStatus
                .Include(ps => ps.Properties)
                .FirstOrDefaultAsync(ps => ps.Id == id);
        }
    }
}
