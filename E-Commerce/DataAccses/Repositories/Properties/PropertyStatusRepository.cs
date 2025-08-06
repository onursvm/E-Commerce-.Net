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
        public async Task<PropertyStatus> AddAsync(PropertyStatus entity)
        {
            await _context.PropertyStatus.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task UpdateAsync(PropertyStatus entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(PropertyStatus entity)
        {
            _context.PropertyStatus.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<PropertyStatus>> GetAllAsync()
        {
            return await _context.PropertyStatus
                .Include(ps => ps.Properties)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PropertyStatus> GetByIdAsync(int id)
        {
            return await _context.PropertyStatus
                .Include(ps => ps.Properties)
                .FirstOrDefaultAsync(ps => ps.Id == id);
        }
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.PropertyStatus
                .AnyAsync(ps => EF.Functions.Like(ps.Name, name));
        }
        public async Task<int> GetPropertyCountByStatusAsync(int statusId)
        {
            return await _context.Properties
                .CountAsync(p => p.PropertyStatusId == statusId);
        }
    }
}
