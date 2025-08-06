using E_Commerce.DataAccses.Entities.Properties;
namespace E_Commerce.DataAccses.Interfaces.Properties
{
    public interface IPropertyStatusRepository
    {
        // CRUD
        Task<PropertyStatus> AddAsync(PropertyStatus entity);
        Task UpdateAsync(PropertyStatus entity);
        Task DeleteAsync(PropertyStatus entity);

        // Okuma
        Task<IEnumerable<PropertyStatus>> GetAllAsync();
        Task<PropertyStatus> GetByIdAsync(int id);
        // Sorgular
        Task<bool> ExistsByNameAsync(string name);
        Task<int> GetPropertyCountByStatusAsync(int statusId);
    }
}
