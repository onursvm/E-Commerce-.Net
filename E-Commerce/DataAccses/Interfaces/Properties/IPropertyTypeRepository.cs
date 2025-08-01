using E_Commerce.DataAccses.Entities.Properties;

namespace E_Commerce.DataAccses.Interfaces.Properties
{
    public interface IPropertyTypeRepository
    {
        Task<IEnumerable<PropertyType>> GetAllAsync();
        Task<PropertyType> GetByIdAsync(int id);
    }
}
