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
        public async Task<PropertyType> AddAsync(PropertyType entity)
        {
            await _context.PropertyTypes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(PropertyType entity)
        {
            _context.PropertyTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PropertyType entity)
        {
            _context.PropertyTypes.Remove(entity);
            await _context.SaveChangesAsync();
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
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.PropertyTypes
                .AnyAsync(pt => EF.Functions.Like(pt.Name, name));
        }

        public async Task<int> GetPropertyCountByTypeAsync(int typeId)
        {
            return await _context.Properties
                .CountAsync(p => p.PropertyTypeId == typeId);
        }

        public async Task<IEnumerable<PropertyType>> SearchAsync(string keyword)
        {
            return await _context.PropertyTypes
                .Where(pt => EF.Functions.Like(pt.Name, $"%{keyword}%"))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
