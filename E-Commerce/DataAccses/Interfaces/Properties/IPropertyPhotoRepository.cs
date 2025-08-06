using E_Commerce.DataAccses.Entities.Properties;
namespace E_Commerce.DataAccses.Interfaces.Properties
{
    public interface IPropertyPhotoRepository
    {
        //Crud
        Task<PropertyPhoto> GetPhotoIdAsync(int id);
        Task<IEnumerable<PropertyPhoto>> GetAllAsync();
        Task<PropertyPhoto> AddAsync(PropertyPhoto entity);
        Task UpdateAsync(PropertyPhoto entity);
        Task DeleteAsync(int id);

        
        //Sorgu
        Task<IEnumerable<PropertyPhoto>> GetPhotosByPropertyAsync(int propertyId);
       Task<PropertyPhoto>GetMainPhotoByPropertyAsync(int propertyId);
        Task<bool> SetAsMainPhotoAsync(int photoId);
        Task<bool> RemoveMainPhotoStatusAsync(int propertyId);
        
        //logic
        Task<bool> ExistsAsync(int id);
        Task<int> CountPhotoByPropertyAsync(int propertyId);
        Task<bool> HasAnyMainPhotoAsync(int propertyId);
    }
}
