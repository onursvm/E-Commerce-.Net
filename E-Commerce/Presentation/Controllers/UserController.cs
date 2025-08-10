using E_Commerce.Business.Manager;
using E_Commerce.DataAccses.Entities.Identity;
using E_Commerce.Presentation.Dtos.Auth;
using E_Commerce.Presentation.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController: ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public UserController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Tüm kullanıcıları getir
        /// </summary>
        /// <returns>Kullanıcı listesi</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllUsers()
        {
            try
            {
                var users = await _serviceManager.UserService.GetAllUsersAsync();
                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var roles = await _serviceManager.RoleService.GetRolesForUserAsync(user.Id);
                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FullName = user.FullName,
                        IsActive = user.IsActive,
                        Roles = roles.Select(r => r.Name).ToList()
                    });
                }

                return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResult(userDtos, "Users retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<UserDto>>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// ID ile kullanıcı getir
        /// </summary>
        /// <param name="id">Kullanıcı ID</param>
        /// <returns>Kullanıcı bilgileri</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(int id)
        {
            try
            {
                var user = await _serviceManager.UserService.GetUserByIdAsync(id);
                var roles = await _serviceManager.RoleService.GetRolesForUserAsync(user.Id);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    Roles = roles.Select(r => r.Name).ToList()
                };

                return Ok(ApiResponse<UserDto>.SuccessResult(userDto, "User retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Yeni kullanıcı oluştur
        /// </summary>
        /// <param name="createUserDto">Kullanıcı bilgileri</param>
        /// <returns>Oluşturulan kullanıcı</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<UserDto>.ErrorResult("Validation failed", errors));
                }

                var newUser = new User
                {
                    UserName = createUserDto.UserName,
                    Email = createUserDto.Email,
                    FullName = createUserDto.FullName,
                    IsActive = createUserDto.IsActive
                };

                var createdUser = await _serviceManager.UserService.CreateUserAsync(newUser, createUserDto.Password);

                // Rolleri ata
                foreach (var roleId in createUserDto.RoleIds)
                {
                    if (await _serviceManager.RoleService.RoleExistsAsync(roleId))
                    {
                        await _serviceManager.RoleService.AssignRoleToUserAsync(createdUser.Id, roleId);
                    }
                }

                var roles = await _serviceManager.RoleService.GetRolesForUserAsync(createdUser.Id);
                var userDto = new UserDto
                {
                    Id = createdUser.Id,
                    UserName = createdUser.UserName,
                    Email = createdUser.Email,
                    FullName = createdUser.FullName,
                    IsActive = createdUser.IsActive,
                    Roles = roles.Select(r => r.Name).ToList()
                };

                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id },
                    ApiResponse<UserDto>.SuccessResult(userDto, "User created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Kullanıcı güncelle
        /// </summary>
        /// <param name="id">Kullanıcı ID</param>
        /// <param name="updateUserDto">Güncellenecek bilgiler</param>
        /// <returns>Güncellenmiş kullanıcı</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<UserDto>.ErrorResult("Validation failed", errors));
                }

                var userToUpdate = new User
                {
                    Id = id,
                    UserName = updateUserDto.UserName,
                    Email = updateUserDto.Email,
                    FullName = updateUserDto.FullName,
                    IsActive = updateUserDto.IsActive
                };

                var updatedUser = await _serviceManager.UserService.UpdateUserAsync(id, userToUpdate);
                var roles = await _serviceManager.RoleService.GetRolesForUserAsync(updatedUser.Id);

                var userDto = new UserDto
                {
                    Id = updatedUser.Id,
                    UserName = updatedUser.UserName,
                    Email = updatedUser.Email,
                    FullName = updatedUser.FullName,
                    IsActive = updatedUser.IsActive,
                    Roles = roles.Select(r => r.Name).ToList()
                };

                return Ok(ApiResponse<UserDto>.SuccessResult(userDto, "User updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Kullanıcı sil
        /// </summary>
        /// <param name="id">Kullanıcı ID</param>
        /// <returns>Silme sonucu</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
        {
            try
            {
                await _serviceManager.UserService.DeleteUserAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(null, "User deleted successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Kullanıcı durumunu değiştir (aktif/pasif)
        /// </summary>
        /// <param name="id">Kullanıcı ID</param>
        /// <param name="statusDto">Durum bilgisi</param>
        /// <returns>Güncelleme sonucu</returns>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> ToggleUserStatus(int id, [FromBody] UserStatusDto statusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Validation failed", errors));
                }

                var result = await _serviceManager.UserService.ToggleUserStatusAsync(id, statusDto.IsActive);
                var message = result ? "User activated successfully" : "User deactivated successfully";

                return Ok(ApiResponse<object>.SuccessResult(new { IsActive = result }, message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Kullanıcıya rol ata
        /// </summary>
        /// <param name="assignRoleDto">Rol atama bilgileri</param>
        /// <returns>Atama sonucu</returns>
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> AssignRoleToUser([FromBody] AssignRoleDto assignRoleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Validation failed", errors));
                }

                await _serviceManager.RoleService.AssignRoleToUserAsync(assignRoleDto.UserId, assignRoleDto.RoleId);
                return Ok(ApiResponse<object>.SuccessResult(null, "Role assigned successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Kullanıcıdan rol kaldır
        /// </summary>
        /// <param name="userId">Kullanıcı ID</param>
        /// <param name="roleId">Rol ID</param>
        /// <returns>Kaldırma sonucu</returns>
        [HttpDelete("{userId}/roles/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveRoleFromUser(int userId, int roleId)
        {
            try
            {
                await _serviceManager.RoleService.RemoveRoleFromUseAsync(userId, roleId);
                return Ok(ApiResponse<object>.SuccessResult(null, "Role removed successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Kullanıcının rollerini getir
        /// </summary>
        /// <param name="id">Kullanıcı ID</param>
        /// <returns>Kullanıcı rolleri</returns>
        [HttpGet("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<string>>>> GetUserRoles(int id)
        {
            try
            {
                var roles = await _serviceManager.RoleService.GetRolesForUserAsync(id);
                var roleNames = roles.Select(r => r.Name).ToList();

                return Ok(ApiResponse<IEnumerable<string>>.SuccessResult(roleNames, "User roles retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<string>>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }
    }
}
