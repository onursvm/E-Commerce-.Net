using E_Commerce.DataAccses.Context;
using E_Commerce.DataAccses.Interfaces.Properties;
using PropertyEntity = E_Commerce.DataAccses.Entities.Properties.Property;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;

namespace E_Commerce.DataAccses.Repositories.Properties
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly CommerceDbContext _context;

        public PropertyRepository(CommerceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PropertyEntity entity)
        {
            entity.StartDate = DateTime.UtcNow;
            await _context.Properties.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CountAsync(Expression<Func<PropertyEntity, bool>> filter = null)
        {
            var query = _context.Properties.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var count = await query.CountAsync();
            return count > 0;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Properties.FindAsync(id);
            if (entity != null)
            {
                _context.Properties.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExitsAsync(int id)
        {
            return await _context.Properties.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PropertyEntity>> GetAllAsync()
        {
            return await _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.PropertyStatus)
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<PropertyEntity> GetByIdAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.PropertyStatus)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PropertyEntity>> GetFilteredAsync(Expression<Func<PropertyEntity, bool>> filter)
        {
            return await _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.PropertyStatus)
                .Include(p => p.Photos)
                .Where(filter)
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyEntity>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<PropertyEntity, bool>> filter = null,
            Func<IQueryable<PropertyEntity>, IOrderedQueryable<PropertyEntity>> orderBy = null)
        {
            var query = _context.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.PropertyStatus)
                .Include(p => p.Photos)
                .AsQueryable();

            // Filter uygula
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Sıralama uygula
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                // Default sıralama: Id'ye göre azalan
                query = query.OrderByDescending(p => p.Id);
            }

            // Sayfalama uygula
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task UpdateAsync(PropertyEntity entity)
        {
            _context.Properties.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}