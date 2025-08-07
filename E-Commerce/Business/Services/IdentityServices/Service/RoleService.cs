using E_Commerce.Business.Services.IdentityServices.Interfaces;
using E_Commerce.DataAccses.Entities.Identity;
using E_Commerce.DataAccses.Interfaces.Identity;

namespace E_Commerce.Business.Services.IdentityServices.Service
{
    public class RoleService:IRoleService
    {

        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task AssignRoleToUserAsync(int userId, int roleId)
        {
            if(!await _roleRepository.RoleExistsAsync(roleId))
                throw new KeyNotFoundException($"Role with ID{roleId} not found");

            if(await _roleRepository.UserHasRoleAsync(userId, roleId))
                throw new InvalidOperationException($"User with ID {userId} already has role {roleId}");

            await _roleRepository.AddRoleToUserAsync(userId, roleId);
        }

        public async Task<Role> CreateRoleAsync(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty");

            if (await _roleRepository.RoleNameExistsAsync(name))
                throw new InvalidOperationException($"Role '{name}' already exists");

            var role = new Role
            {
                Name = name,
                Description = description
            };

            return await _roleRepository.AddAsync(role);
        }

        public async Task DeleteRoleAsync(int roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new KeyNotFoundException($"Role with ID {roleId} not found");

            // Önce rolün kullanıcılardan kaldırılması gerekebilir
            var usersWithRole = await _roleRepository.GetUsersInRoleAsync(roleId);
            foreach (var user in usersWithRole)
            {
                await _roleRepository.RemoveRoleFromUserAsync(user.Id, roleId);
            }

            await _roleRepository.DeleteAsync(role);
        }

        public async Task<IEnumerable<Role>> GetAllRoleAsync()
        {
            return await _roleRepository.GetAllAsync();
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return role ?? throw new KeyNotFoundException($"Role with ID {id} not found");
        }

        public async Task<IEnumerable<Role>> GetRolesForUserAsync(int userId)
        {
            return await _roleRepository.GetUserRolesAsync(userId);
        }

        public async Task RemoveRoleFromUseAsync(int userId, int roleId)
        {
            if (!await _roleRepository.UserHasRoleAsync(userId, roleId))
                throw new InvalidOperationException("User does not have this role");

            await _roleRepository.RemoveRoleFromUserAsync(userId, roleId);
        }

        public async Task<bool> RoleExistsAsync(int roleId)
        {
            return await _roleRepository.RoleExistsAsync(roleId);
        }

        public async Task<bool> RoleNameExistsAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return await _roleRepository.RoleNameExistsAsync(name);
        }

        public async Task<Role> UpdateRoleAsync(int roleId, string newRoleName, string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newRoleName))
                throw new ArgumentException("Role name cannot be empty");

            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new KeyNotFoundException($"Role with ID {roleId} not found");

            if (role.Name != newRoleName && await _roleRepository.RoleNameExistsAsync(newRoleName))
                throw new InvalidOperationException($"Role '{newRoleName}' already exists");

            role.Name = newRoleName;
            role.Description = newDescription;

            await _roleRepository.UpdateAsync(role);
            return role;
        }

        

        public async Task<bool> UserHasRoleAsync(int userId, int roleId)
        {
            return await _roleRepository.UserHasRoleAsync(userId, roleId);
        }
    }
}
