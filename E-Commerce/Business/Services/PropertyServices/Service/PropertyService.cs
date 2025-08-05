using E_Commerce.Business.Services.PropertyServices.Interfaces;
using E_Commerce.DataAccses.Entities.Properties;
using E_Commerce.DataAccses.Interfaces.Properties;

namespace E_Commerce.Business.Services.PropertyServices.Service
{
    public class PropertyService:IPropertyService
    {
        private readonly IPropertyRepository _repository;
        private readonly IPropertyPhotoService _photoService;
        private readonly IPropertyTypeService _propertyTypeService;
        public PropertyService(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public IPropertyPhotoService PhotoService => throw new NotImplementedException();

        public IPropertyTypeService PropertyTypeService => throw new NotImplementedException();

        public IPropertyStatusService PropertyStatusService => throw new NotImplementedException();

        public Task<decimal> CalculateTotalRevenueAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Property> CreateAsync(Property property)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetActivePropertyCountAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Property>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Property>> GetAvailablePropertiesAsync(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<Property> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Property>> GetByLocationAsync(string location)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Property>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Property>> GetByStatusAsync(int statusId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Property>> GetByTypeAsync(int typeId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Property property)
        {
            throw new NotImplementedException();
        }
    }
}
