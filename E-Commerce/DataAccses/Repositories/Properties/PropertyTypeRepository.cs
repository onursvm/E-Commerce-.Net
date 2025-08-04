using E_Commerce.DataAccses.Context;
using E_Commerce.DataAccses.Entities.Properties;
using E_Commerce.DataAccses.Interfaces.Properties;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccses.Repositories.Properties
{
    public class PropertyTypeRepository : IPropertyTypeRepository
    {
        private readonly CommerceDbContext _context;
        public PropertyTypeRepository(CommerceDbContext context) 
        {
            _context = context;
        }
        
        public async Task<IEnumerable<PropertyType>> GetAllAsync()
        {
            return await _context.PropertyTypes
                .Include(pt => pt.Properties)
                .OrderBy(pt => pt.Name)
                .ToListAsync();
        }

        public async Task<PropertyType> GetByIdAsync(int id)
        {
            return await _context.PropertyTypes
                .Include(pt => pt.Properties)
                .FirstOrDefaultAsync(pt => pt.Id == id);
        }
    }
}
