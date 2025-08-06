using E_Commerce.DataAccses.Entities.Properties;

namespace E_Commerce.DataAccses.Interfaces.Properties
{
    public interface IPropertyTypeRepository
    {
        Task<PropertyType> AddAsync(PropertyType entity);
        Task UpdateAsync(PropertyType entity);
        Task DeleteAsync(PropertyType entity);
        Task<IEnumerable<PropertyType>> GetAllAsync();
        Task<PropertyType> GetByIdAsync(int id);

        Task<bool> ExistsByNameAsync(string name);
        Task<int> GetPropertyCountByTypeAsync(int typeId);
        Task<IEnumerable<PropertyType>> SearchAsync(string keyword);
    }
}
