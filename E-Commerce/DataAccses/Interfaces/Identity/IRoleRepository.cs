using E_Commerce.DataAccses.Entities.Identity;
namespace E_Commerce.DataAccses.Interfaces.Identity
{
    public interface IRoleRepository
    {
        Task<Role> AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);

        // Sorgu Operasyonları
        Task<Role> GetByIdAsync(int id);
        Task<Role> GetByNameAsync(string name);
        Task<IEnumerable<Role>> GetAllAsync();

        // Rol-Varlık İlişkileri
        Task AddRoleToUserAsync(int userId, int roleId);
        Task RemoveRoleFromUserAsync(int userId, int roleId);
        Task<bool> UserHasRoleAsync(int userId, int roleId);
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
        Task<IEnumerable<User>> GetUsersInRoleAsync(int roleId);

        // Validasyon Metodları
        Task<bool> RoleExistsAsync(int roleId);
        Task<bool> RoleNameExistsAsync(string name);
    }
}
