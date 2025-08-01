using E_Commerce.DataAccses.Entities.Properties;
namespace E_Commerce.DataAccses.Interfaces.Properties
{
    public interface IPropertyStatusRepository
    {
        Task<IEnumerable<PropertyStatus>> GetAllAsync();
        Task<PropertyStatus> GetByIdAsync(int id);
    }
}
