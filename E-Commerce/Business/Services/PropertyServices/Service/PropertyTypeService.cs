using E_Commerce.Business.Services.PropertyServices.Interfaces;
using E_Commerce.DataAccses.Entities.Properties;
using E_Commerce.DataAccses.Interfaces.Properties;
using E_Commerce.DataAccses.Repositories.Properties;

namespace E_Commerce.Business.Services.PropertyServices.Service
{
    public class PropertyTypeService:IPropertyTypeService
    {
        private readonly IPropertyTypeRepository _typeRepository;
        public PropertyTypeService(IPropertyTypeRepository typeRepository)
        {
            _typeRepository = typeRepository;
        }

        public async Task<PropertyType> CreateAsync(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException("Type name cannot be empty");

            if (await _typeRepository.ExistsByNameAsync(typeName))
                throw new InvalidOperationException($"Type '{typeName}' already exists");

            var newType = new PropertyType { Name = typeName };
            return await _typeRepository.AddAsync(newType);
        }

        public async Task DeleteAsync(int id)
        {
            var type = await _typeRepository.GetByIdAsync(id);
            if (type == null)
                throw new KeyNotFoundException($"Type with ID {id} not found");

            if (await _typeRepository.GetPropertyCountByTypeAsync(id) > 0)
                throw new InvalidOperationException("Cannot delete type in use");

            await _typeRepository.DeleteAsync(type);
        }

        public async Task<bool> ExistsByNameAsync(string typeName)
        {
            return await _typeRepository.ExistsByNameAsync(typeName);
        }

        public async Task<IEnumerable<PropertyType>> GetAllAsync()
        {
            return await _typeRepository.GetAllAsync();
        }

        public async Task<int> GetPropertyCountByTypeAsync(int typeId)
        {
            return await _typeRepository.GetPropertyCountByTypeAsync(typeId);
        }

        public async Task<PropertyType> GetTypeIdAsync(int id)
        {
            var type = await _typeRepository.GetByIdAsync(id);
            return type ?? throw new KeyNotFoundException($"Type with ID {id} not found");
        }

        public async Task<bool> IsTypeInUseAsync(int typeId)
        {
            return await _typeRepository.GetPropertyCountByTypeAsync(typeId) > 0;
        }

        public async Task<IEnumerable<PropertyType>> SearchTypeAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await _typeRepository.GetAllAsync();

            return await _typeRepository.SearchAsync(keyword);
        }

        public async Task UpdateAsync(int id, string newTypeName)
        {
            if (string.IsNullOrWhiteSpace(newTypeName))
                throw new ArgumentException("Type name cannot be empty");

            var type = await _typeRepository.GetByIdAsync(id);
            if (type == null)
                throw new KeyNotFoundException($"Type with ID {id} not found");

            if (await _typeRepository.ExistsByNameAsync(newTypeName))
                throw new InvalidOperationException($"Type '{newTypeName}' already exists");

            type.Name = newTypeName;
            await _typeRepository.UpdateAsync(type);
        }

        public async Task<bool> ValidateTypeName(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return false;

            return !await _typeRepository.ExistsByNameAsync(typeName);
        }
    }
}
