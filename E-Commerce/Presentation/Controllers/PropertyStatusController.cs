using E_Commerce.Business.Manager;
using E_Commerce.Presentation.Dtos.Auth;
using E_Commerce.Presentation.Dtos.PropertyStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyStatusController: ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public PropertyStatusController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Tüm property statuslarını getir
        /// </summary>
        /// <returns>Property status listesi</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<PropertyStatusDto>>>> GetAllPropertyStatuses()
        {
            try
            {
                var propertyStatuses = await _serviceManager.PropertyService.PropertyStatusService.GetAllAsync();
                var propertyStatusDtos = new List<PropertyStatusDto>();

                foreach (var status in propertyStatuses)
                {
                    var propertyCount = await _serviceManager.PropertyService.PropertyStatusService.GetPropetyCountByStatusAsync(status.Id);
                    propertyStatusDtos.Add(new PropertyStatusDto
                    {
                        Id = status.Id,
                        Name = status.Name,
                        PropertyCount = propertyCount
                    });
                }

                return Ok(ApiResponse<IEnumerable<PropertyStatusDto>>.SuccessResult(propertyStatusDtos, "Property statuses retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<PropertyStatusDto>>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// ID ile property status getir
        /// </summary>
        /// <param name="id">Property status ID</param>
        /// <returns>Property status bilgileri</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PropertyStatusDto>>> GetPropertyStatusById(int id)
        {
            try
            {
                var propertyStatus = await _serviceManager.PropertyService.PropertyStatusService.GetBYIdAsync(id);
                var propertyCount = await _serviceManager.PropertyService.PropertyStatusService.GetPropetyCountByStatusAsync(id);

                var propertyStatusDto = new PropertyStatusDto
                {
                    Id = propertyStatus.Id,
                    Name = propertyStatus.Name,
                    PropertyCount = propertyCount
                };

                return Ok(ApiResponse<PropertyStatusDto>.SuccessResult(propertyStatusDto, "Property status retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PropertyStatusDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertyStatusDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Yeni property status oluştur
        /// </summary>
        /// <param name="createPropertyStatusDto">Property status bilgileri</param>
        /// <returns>Oluşturulan property status</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<PropertyStatusDto>>> CreatePropertyStatus([FromBody] CreatePropertyStatusDto createPropertyStatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<PropertyStatusDto>.ErrorResult("Validation failed", errors));
                }

                var createdPropertyStatus = await _serviceManager.PropertyService.PropertyStatusService.CreateAsync(createPropertyStatusDto.Name);
                var propertyStatusDto = new PropertyStatusDto
                {
                    Id = createdPropertyStatus.Id,
                    Name = createdPropertyStatus.Name,
                    PropertyCount = 0
                };

                return CreatedAtAction(nameof(GetPropertyStatusById), new { id = createdPropertyStatus.Id },
                    ApiResponse<PropertyStatusDto>.SuccessResult(propertyStatusDto, "Property status created successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<PropertyStatusDto>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PropertyStatusDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertyStatusDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property status güncelle
        /// </summary>
        /// <param name="id">Property status ID</param>
        /// <param name="updatePropertyStatusDto">Güncellenecek bilgiler</param>
        /// <returns>Güncellenmiş property status</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<PropertyStatusDto>>> UpdatePropertyStatus(int id, [FromBody] UpdatePropertyStatusDto updatePropertyStatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<PropertyStatusDto>.ErrorResult("Validation failed", errors));
                }

                await _serviceManager.PropertyService.PropertyStatusService.UpdateAsync(id, updatePropertyStatusDto.Name);
                var updatedPropertyStatus = await _serviceManager.PropertyService.PropertyStatusService.GetBYIdAsync(id);
                var propertyCount = await _serviceManager.PropertyService.PropertyStatusService.GetPropetyCountByStatusAsync(id);

                var propertyStatusDto = new PropertyStatusDto
                {
                    Id = updatedPropertyStatus.Id,
                    Name = updatedPropertyStatus.Name,
                    PropertyCount = propertyCount
                };

                return Ok(ApiResponse<PropertyStatusDto>.SuccessResult(propertyStatusDto, "Property status updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<PropertyStatusDto>.ErrorResult(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PropertyStatusDto>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PropertyStatusDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertyStatusDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property status sil
        /// </summary>
        /// <param name="id">Property status ID</param>
        /// <returns>Silme sonucu</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeletePropertyStatus(int id)
        {
            try
            {
                await _serviceManager.PropertyService.PropertyStatusService.DeleteAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(null, "Property status deleted successfully"));
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
        /// Property status silinebilirlik kontrolü
        /// </summary>
        /// <param name="id">Property status ID</param>
        /// <returns>Silinebilirlik durumu</returns>
        [HttpGet("{id}/can-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> CanDeletePropertyStatus(int id)
        {
            try
            {
                var canDelete = await _serviceManager.PropertyService.PropertyStatusService.CanDeleteStatusAsync(id);
                return Ok(ApiResponse<bool>.SuccessResult(canDelete, "Property status deletion eligibility checked"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property status adı varlık kontrolü
        /// </summary>
        /// <param name="name">Status adı</param>
        /// <returns>Varlık durumu</returns>
        [HttpGet("exists-by-name")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> PropertyStatusExistsByName([FromQuery] string name)
        {
            try
            {
                var exists = await _serviceManager.PropertyService.PropertyStatusService.ExistsByNameAsync(name);
                return Ok(ApiResponse<bool>.SuccessResult(exists, "Property status name existence checked"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }
    }
}
