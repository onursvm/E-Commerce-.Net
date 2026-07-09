using E_Commerce.Business.Services.PropertyServices.Interfaces;
using E_Commerce.DataAccses.Entities.Properties;
using E_Commerce.DataAccses.Interfaces.Properties;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Business.Services.PropertyServices.Service
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _repository;
        private readonly IPropertyPhotoService _photoService;
        private readonly IPropertyTypeService _typeService;
        private readonly IPropertyStatusService _statusService;
        public PropertyService(
            IPropertyRepository repository,
            IPropertyPhotoService photoService,
            IPropertyTypeService typeService,
            IPropertyStatusService statusService)
        {
            _repository = repository;
            _photoService = photoService;
            _typeService = typeService;
            _statusService = statusService;

        }

        public IPropertyPhotoService PhotoService => _photoService;

        public IPropertyTypeService PropertyTypeService => _typeService;

        public IPropertyStatusService PropertyStatusService => _statusService;

        public async Task<decimal> CalculateTotalRevenueAsync()
        {
            var properties = await _repository.GetFilteredAsync(p => p.EndDate < DateTime.Now);
            return properties.Sum(p => p.Price);
        }

        public async Task<Property> CreateAsync(Property property)
        {
            if (property.Price <= 0)
                throw new ArgumentException("Price must be greater than zero");

            if (property.EndDate <= property.StartDate)
                throw new ArgumentException("End date cannot be after start date");

            var properType = await _typeService.GetTypeIdAsync(property.PropertyTypeId);
            if (properType == null)
                throw new KeyNotFoundException("Property type not found");

            return await _repository.AddAsync(property);
        }

        public async Task DeleteAsync(int id)
        {
            var property = await _repository.GetByIdAsync(id);
            if (property == null)
                throw new KeyNotFoundException("Property not found");
            var photos = await _photoService.GetPhotosBypPropertyAsync(id);
            if (photos.Any())
                throw new InvalidOperationException("Cannot delete property with existing photos");

            await _repository.DeleteAsync(id);
        }

        public async Task<int> GetActivePropertyCountAsync()
        {
            var activeStatuses = await _statusService.GetBYIdAsync();
            var activeStatusIds = activeStatuses.Select(s => s.Id).ToList();

            return await _repository.CountAsync(p =>
            activeStatusIds.Contains(p.PropertyStatusId) &&
            p.EndDate > DateTime.Now);
        }

        public Task<IEnumerable<Property>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public async Task<IEnumerable<Property>> GetAvailablePropertiesAsync(DateTime startDate, DateTime endDate)
        {
            return await _repository.GetFilteredAsync(p =>
                p.StartDate <= endDate && p.EndDate <= startDate);
        }

        public async Task<Property> GetByIdAsync(int id)
        {
            var property = await _repository.GetByIdAsync(id);
            return property ?? throw new KeyNotFoundException("Property not found");
        }

        public async Task<IEnumerable<Property>> GetByLocationAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return Enumerable.Empty<Property>();

            return await _repository.GetFilteredAsync(p =>
            p.Location.Contains(location, StringComparison.OrdinalIgnoreCase));

        }

        public async Task<IEnumerable<Property>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice > maxPrice)
                throw new ArgumentException("Minimum price cannot be greater than maximum price");

            return await _repository.GetFilteredAsync(p =>
                p.Price >= minPrice && p.Price <= maxPrice);
        }

        public async Task<IEnumerable<Property>> GetByStatusAsync(int statusId)
        {
            return await _repository.GetFilteredAsync(p =>
            p.PropertyStatusId == statusId);
        }

        public async Task<IEnumerable<Property>> GetByTypeAsync(int typeId)
        {
            return await _repository.GetFilteredAsync(p =>
            p.PropertyTypeId == typeId);
        }

        public async Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime startDate, DateTime endDate)
        {
            var property = await _repository.GetByIdAsync(propertyId);
            if (property == null) return false;

            return property.StartDate <= endDate && property.EndDate >= startDate;
        }

        public async Task UpdateAsync(Property property)
        {
            var existing = await _repository.GetByIdAsync(property.Id);
            if (existing == null)
                throw new KeyNotFoundException("Property not found");
            property.PropertyTypeId = existing.PropertyTypeId;
            property.PropertyStatusId = existing.PropertyStatusId;

            await _repository.UpdateAsync(property);
        }
    }
}