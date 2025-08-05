using E_Commerce.DataAccses.Entities.Properties;

namespace E_Commerce.Business.Services.PropertyServices.Interfaces
{
    public interface IPropertyService
    {
        // CRUD Operations
        Task<Property>CreateAsync (Property property);
        Task UpdateAsync (Property property);
        Task DeleteAsync (int id);
        Task<Property> GetByIdAsync (int id);
        Task<IEnumerable<Property>> GetAllAsync ();

        //Filttreleme
        Task<IEnumerable<Property>> GetByTypeAsync(int typeId);
        Task<IEnumerable<Property>> GetByStatusAsync(int statusId);
        Task<IEnumerable<Property>> GetByLocationAsync(string location);
        Task<IEnumerable<Property>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        

        //Business Logic
        Task<IEnumerable<Property>> GetAvailablePropertiesAsync(DateTime startDate, DateTime endDate);
        Task <bool> IsPropertyAvailableAsync(int propertyId, DateTime startDate, DateTime endDate);
        Task<int> GetActivePropertyCountAsync();    // Aktif property sayısı (dashboard için)
        Task<decimal> CalculateTotalRevenueAsync(); // Toplam ciro (raporlama için)

        //Composite Services
        IPropertyPhotoService PhotoService { get; }
        IPropertyTypeService  PropertyTypeService { get; }
        IPropertyStatusService PropertyStatusService { get; }



    }
}
