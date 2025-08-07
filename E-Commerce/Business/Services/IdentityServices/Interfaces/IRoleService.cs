using E_Commerce.DataAccses.Entities.Identity;

namespace E_Commerce.Business.Services.IdentityServices.Interfaces
{
    public interface IRoleService
    {
        // CRUD Operations
        Task <Role>CreateRoleAsync(string name,string descrription);
        Task<Role>UpdateRoleAsync(int roleId, string newRoleName,string newDescription);
        Task DeleteRoleAsync(int roleName);

        //Okuma
        Task<Role> GetRoleByIdAsync(int id);
        Task<IEnumerable<Role>>GetAllRoleAsync();
        Task<IEnumerable<Role>> GetRolesForUserAsync(int userId);

        // Rol Yönetimi
        Task AssignRoleToUserAsync(int userId,int roleId);
        Task RemoveRoleFromUseAsync(int userId, int roleId);
        Task<bool> UserHasRoleAsync(int userId,int  roleId);
        Task <bool>RoleExistsAsync(int roleId);
        Task<bool> RoleNameExistsAsync(string name);


       
    }
}
