using E_Commerce.Business.Services.PropertyServices.Interfaces;
using E_Commerce.DataAccses.Entities.Properties;
using E_Commerce.DataAccses.Interfaces.Properties;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace E_Commerce.Business.Services.PropertyServices.Service
{
    public class PropertyPhotoService:IPropertyPhotoService
    {
        private readonly IPropertyPhotoRepository _photoRepository;

        public PropertyPhotoService(IPropertyPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }
        public async Task<PropertyPhoto> AddPhotoAsync(int propertyId, string imageUrl, bool isMain)
        {
            var photo = new PropertyPhoto
            {
                PropertyId = propertyId,
                Url = imageUrl,
                IsMain = isMain.ToString(),
                
            };
            return await _photoRepository.AddAsync(photo);
        }

        public async Task DeletePhotoAsync(int photoId)
        {
            try
            {
                if (!await _photoRepository.ExistsAsync(photoId))
                    throw new KeyNotFoundException($"Photo with ID {photoId} not found");

                await _photoRepository.DeleteAsync(photoId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting photo with ID {photoId}: {ex.Message}", ex);
            }
        }

        public Task<string> GenerateUploadUrlAsync(int propertyId)
        {
            throw new NotImplementedException("enerateUploadUrlAsync bulut depolama " +
                "sağlayıcınıza göre implemente edilmelidir");
        }

        public async Task<int> GetPhotoCountForPopertyAsync(int properId)
        {
            return await _photoRepository.CountPhotoByPropertyAsync(properId);
        }

        public async Task<PropertyPhoto> GetPhotoIdAsync(int photoId)
        {
            var photo = await _photoRepository.GetPhotoIdAsync(photoId);
            return photo ?? throw new KeyNotFoundException($"Photo with ID {photoId} not found");
        }

        public async Task<IEnumerable<PropertyPhoto>> GetPhotosBypPropertyAsync(int propertyId)
        {
            return await _photoRepository.GetPhotosByPropertyAsync(propertyId);
        }

        public async Task<bool> IsPhotoOwnedByUserAsync(int photoId, int userId)
        {
            var photo = await _photoRepository.GetPhotoIdAsync(photoId);
            return photo?.Property?.UserId == userId;
        }

        public async Task RotatePhotoAsync(int propertyId, int photoId)
        {
            var photo = await _photoRepository.GetPhotoIdAsync(photoId);
            if (photo == null || photo.PropertyId != propertyId)
            {
                throw new ArgumentException("Invalid photo ID for this property");
            }

            // Remove main status from current main photo
            await _photoRepository.RemoveMainPhotoStatusAsync(propertyId);
        }

        public async Task SetMainPhotoAsync(int propertyId, int photoId)
        {
            var photo = await _photoRepository.GetPhotoIdAsync(photoId);
            if (photo == null)
                throw new KeyNotFoundException($"Photo {photoId} not found");

            if(photo.PropertyId != propertyId)
        throw new ArgumentException($"Photo {photoId} does not belong to property {propertyId}");

            // 2. Mevcut ana fotoğrafı kaldır
            await _photoRepository.RemoveMainPhotoStatusAsync(propertyId);

            // 3. Yeni ana fotoğrafı ayarla
            if (!await _photoRepository.SetAsMainPhotoAsync(photoId))
                throw new Exception("Failed to update photo status");
        }
    }
}
