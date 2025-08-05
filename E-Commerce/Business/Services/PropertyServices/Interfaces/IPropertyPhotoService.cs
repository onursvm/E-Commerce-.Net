
using E_Commerce.DataAccses.Entities.Properties;

namespace E_Commerce.Business.Services.PropertyServices.Interfaces
{
    public interface IPropertyPhotoService
    {
        //Crud
        Task<PropertyPhoto>GetPhotoIdAsync(int photoId);
        Task<IEnumerable<PropertyPhoto>> GetPhotosBypPropertyAsync(int propertyId);
        Task<PropertyPhoto> AddPhotoAsync(int propertyId,string imageUrl,bool isMain);
        Task DeletePhotoAsync(int photoId);

        //Sorgu
        Task SetMainPhotoAsync(int propertyId,int photoId);
        Task RotatePhotoAsync(int photoId,int degress);
        Task <string> GenerateUploadUrlAsync(int propertyId);


        //logic
        Task<bool>IsPhotoOwnedByUserAsync(int photoId,int userId);
        Task<int>GetPhotoCountForPopertyAsync(int properId);


    }
}
