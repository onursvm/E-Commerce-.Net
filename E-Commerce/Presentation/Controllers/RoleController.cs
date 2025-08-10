using E_Commerce.Business.Manager;
using E_Commerce.Presentation.Dtos.Auth;
using E_Commerce.Presentation.Dtos.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController: ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public RoleController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Tüm rolleri getir
        /// </summary>
        /// <returns>Rol listesi</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleDto>>>> GetAllRoles()
        {
            try
            {
                var roles = await _serviceManager.RoleService.GetAllRoleAsync();
                var roleDtos = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                }).ToList();

                return Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResult(roleDtos, "Roles retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<RoleDto>>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// ID ile rol getir
        /// </summary>
        /// <param name="id">Rol ID</param>
        /// <returns>Rol bilgileri</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> GetRoleById(int id)
        {
            try
            {
                var role = await _serviceManager.RoleService.GetRoleByIdAsync(id);
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description
                };

                return Ok(ApiResponse<RoleDto>.SuccessResult(roleDto, "Role retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<RoleDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<RoleDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Yeni rol oluştur
        /// </summary>
        /// <param name="createRoleDto">Rol bilgileri</param>
        /// <returns>Oluşturulan rol</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<RoleDto>.ErrorResult("Validation failed", errors));
                }

                var createdRole = await _serviceManager.RoleService.CreateRoleAsync(createRoleDto.Name, createRoleDto.Description);
                var roleDto = new RoleDto
                {
                    Id = createdRole.Id,
                    Name = createdRole.Name,
                    Description = createdRole.Description
                };

                return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id },
                    ApiResponse<RoleDto>.SuccessResult(roleDto, "Role created successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<RoleDto>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RoleDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<RoleDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Rol güncelle
        /// </summary>
        /// <param name="id">Rol ID</param>
        /// <param name="updateRoleDto">Güncellenecek bilgiler</param>
        /// <returns>Güncellenmiş rol</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<RoleDto>.ErrorResult("Validation failed", errors));
                }

                var updatedRole = await _serviceManager.RoleService.UpdateRoleAsync(id, updateRoleDto.Name, updateRoleDto.Description);
                var roleDto = new RoleDto
                {
                    Id = updatedRole.Id,
                    Name = updatedRole.Name,
                    Description = updatedRole.Description
                };

                return Ok(ApiResponse<RoleDto>.SuccessResult(roleDto, "Role updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<RoleDto>.ErrorResult(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<RoleDto>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RoleDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<RoleDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Rol sil
        /// </summary>
        /// <param name="id">Rol ID</param>
        /// <returns>Silme sonucu</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteRole(int id)
        {
            try
            {
                await _serviceManager.RoleService.DeleteRoleAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(null, "Role deleted successfully"));
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
        /// Rol varlık kontrolü
        /// </summary>
        /// <param name="id">Rol ID</param>
        /// <returns>Varlık durumu</returns>
        [HttpGet("{id}/exists")]
        public async Task<ActionResult<ApiResponse<bool>>> RoleExists(int id)
        {
            try
            {
                var exists = await _serviceManager.RoleService.RoleExistsAsync(id);
                return Ok(ApiResponse<bool>.SuccessResult(exists, "Role existence checked"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Rol adı varlık kontrolü
        /// </summary>
        /// <param name="name">Rol adı</param>
        /// <returns>Varlık durumu</returns>
        [HttpGet("check-name/{name}")]
        public async Task<ActionResult<ApiResponse<bool>>> RoleNameExists(string name)
        {
            try
            {
                var exists = await _serviceManager.RoleService.RoleNameExistsAsync(name);
                return Ok(ApiResponse<bool>.SuccessResult(exists, "Role name existence checked"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }
    }
}
