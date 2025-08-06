using E_Commerce.DataAccses.Context;
using E_Commerce.DataAccses.Entities.Properties;
using E_Commerce.DataAccses.Interfaces.Properties;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccses.Repositories.Properties
{
    public class PropertyPhotoRepository : IPropertyPhotoRepository
    {
        private readonly CommerceDbContext _context;
        public PropertyPhotoRepository(CommerceDbContext context)
        {
            _context = context;
        }
        public async Task<PropertyPhoto> AddAsync(PropertyPhoto entity)
        {
            await _context.PropertyPhotes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<int> CountPhotoByPropertyAsync(int propertyId)
        {
            return await _context.PropertyPhotes
                  .CountAsync(p => p.PropertyId == propertyId);
        }

        public async Task DeleteAsync(int id)
        {
            var photo=await _context.PropertyPhotes.FindAsync(id);
            if (photo != null)
            {
                _context.PropertyPhotes.Remove(photo);
                await _context.SaveChangesAsync();
            }
            }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.PropertyPhotes
                .AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PropertyPhoto>> GetAllAsync()
        {
            return await _context.PropertyPhotes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PropertyPhoto> GetMainPhotoByPropertyAsync(int propertyId)
        {
            return await _context.PropertyPhotes
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId &&
                p.IsMain.Equals("true",StringComparison.OrdinalIgnoreCase));
        }

        public async Task<PropertyPhoto> GetPhotoIdAsync(int id)
        {
            return await _context.PropertyPhotes
                .Include(p => p.Property)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PropertyPhoto>> GetPhotosByPropertyAsync(int propertyId)
        {
            return await _context.PropertyPhotes
                .Where(p => p.PropertyId == propertyId)
                .ToListAsync(); ;
        }

        public async Task<bool> HasAnyMainPhotoAsync(int propertyId)
        {
            return await _context.PropertyPhotes
                .AnyAsync(p => p.PropertyId == propertyId &&
                              p.IsMain.Equals("true", StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> RemoveMainPhotoStatusAsync(int propertyId)
        {
            var mainPhoto = await GetMainPhotoByPropertyAsync(propertyId);
            if (mainPhoto == null) return false;

            mainPhoto.IsMain = "false";
            _context.PropertyPhotes.Update(mainPhoto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetAsMainPhotoAsync(int photoId)
        {
            var photo = await _context.PropertyPhotes.FindAsync(photoId);
            if (photo == null) return false;

            // Önceki ana fotoğrafı kaldır
            await RemoveMainPhotoStatusAsync(photo.PropertyId);

            // Yeni ana fotoğrafı ayarla
            photo.IsMain = "true";
            _context.PropertyPhotes.Update(photo);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task UpdateAsync(PropertyPhoto entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
