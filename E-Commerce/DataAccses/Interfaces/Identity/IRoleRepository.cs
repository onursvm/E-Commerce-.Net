using E_Commerce.DataAccses.Entities.Identity;
namespace E_Commerce.DataAccses.Interfaces.Identity
{
    public interface IRoleRepository
    {
        Task<Role> GetByIdAsync(int id);
        Task<Role> GetByNameAsync(string name);
        Task<IEnumerable<Role>> GetAllAsync();
    }
}
