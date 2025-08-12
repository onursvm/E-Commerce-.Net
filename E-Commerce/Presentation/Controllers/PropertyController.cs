using E_Commerce.Business.Manager;
using E_Commerce.DataAccses.Entities.Properties;
using E_Commerce.Presentation.Dtos.Auth;
using E_Commerce.Presentation.Dtos.Property;
using E_Commerce.Presentation.Dtos.PropertyPhoto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyController: ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public PropertyController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Tüm properties getir
        /// </summary>
        /// <returns>Property listesi</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<PropertyDto>>>> GetAllProperties()
        {
            try
            {
                var properties = await _serviceManager.PropertyService.GetAllAsync();
                var propertyDtos = await MapToPropertyDtos(properties);

                return Ok(ApiResponse<IEnumerable<PropertyDto>>.SuccessResult(propertyDtos, "Properties retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<PropertyDto>>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Filtrelenmiş properties getir
        /// </summary>
        /// <param name="filterDto">Filtre kriterleri</param>
        /// <returns>Filtrelenmiş property listesi</returns>
        [HttpPost("filter")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<PropertyDto>>>> GetFilteredProperties([FromBody] PropertyFilterDto filterDto)
        {
            try
            {
                var properties = new List<Property>();

                if (filterDto.PropertyTypeId.HasValue)
                {
                    var typeProperties = await _serviceManager.PropertyService.GetByTypeAsync(filterDto.PropertyTypeId.Value);
                    properties.AddRange(typeProperties);
                }
                else if (filterDto.PropertyStatusId.HasValue)
                {
                    var statusProperties = await _serviceManager.PropertyService.GetByStatusAsync(filterDto.PropertyStatusId.Value);
                    properties.AddRange(statusProperties);
                }
                else if (!string.IsNullOrEmpty(filterDto.Location))
                {
                    var locationProperties = await _serviceManager.PropertyService.GetByLocationAsync(filterDto.Location);
                    properties.AddRange(locationProperties);
                }
                else if (filterDto.MinPrice.HasValue || filterDto.MaxPrice.HasValue)
                {
                    var minPrice = filterDto.MinPrice ?? 0;
                    var maxPrice = filterDto.MaxPrice ?? decimal.MaxValue;
                    var priceProperties = await _serviceManager.PropertyService.GetByPriceRangeAsync(minPrice, maxPrice);
                    properties.AddRange(priceProperties);
                }
                else
                {
                    var allProperties = await _serviceManager.PropertyService.GetAllAsync();
                    properties.AddRange(allProperties);
                }

                // Pagination
                var skip = (filterDto.PageNumber - 1) * filterDto.PageSize;
                var pagedProperties = properties.Skip(skip).Take(filterDto.PageSize);

                var propertyDtos = await MapToPropertyDtos(pagedProperties);

                return Ok(ApiResponse<IEnumerable<PropertyDto>>.SuccessResult(propertyDtos, "Filtered properties retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<PropertyDto>>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// ID ile property getir
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <returns>Property bilgileri</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PropertyDto>>> GetPropertyById(int id)
        {
            try
            {
                var property = await _serviceManager.PropertyService.GetByIdAsync(id);
                var propertyDto = await MapToPropertyDto(property);

                return Ok(ApiResponse<PropertyDto>.SuccessResult(propertyDto, "Property retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PropertyDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertyDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Yeni property oluştur
        /// </summary>
        /// <param name="createPropertyDto">Property bilgileri</param>
        /// <returns>Oluşturulan property</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PropertyDto>>> CreateProperty([FromBody] CreatePropertyDto createPropertyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<PropertyDto>.ErrorResult("Validation failed", errors));
                }

                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var newProperty = new Property
                {
                    Title = createPropertyDto.Title,
                    Description = createPropertyDto.Description,
                    Price = createPropertyDto.Price,
                    Currency = createPropertyDto.Currency,
                    EndDate = createPropertyDto.EndDate,
                    Location = createPropertyDto.Location,
                    UserId = currentUserId, // Giriş yapmış kullanıcı
                    PropertyTypeId = createPropertyDto.PropertyTypeId,
                    PropertyStatusId = createPropertyDto.PropertyStatusId
                };

                var createdProperty = await _serviceManager.PropertyService.CreateAsync(newProperty);
                var propertyDto = await MapToPropertyDto(createdProperty);

                return CreatedAtAction(nameof(GetPropertyById), new { id = createdProperty.Id },
                    ApiResponse<PropertyDto>.SuccessResult(propertyDto, "Property created successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<PropertyDto>.ErrorResult(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ApiResponse<PropertyDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertyDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property güncelle
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <param name="updatePropertyDto">Güncellenecek bilgiler</param>
        /// <returns>Güncellenmiş property</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<PropertyDto>>> UpdateProperty(int id, [FromBody] UpdatePropertyDto updatePropertyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<PropertyDto>.ErrorResult("Validation failed", errors));
                }

                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var existingProperty = await _serviceManager.PropertyService.GetByIdAsync(id);

                // Sadece property sahibi veya admin güncelleyebilir
                if (existingProperty.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var updatedProperty = new Property
                {
                    Id = id,
                    Title = updatePropertyDto.Title,
                    Description = updatePropertyDto.Description,
                    Price = updatePropertyDto.Price,
                    Currency = updatePropertyDto.Currency,
                    EndDate = updatePropertyDto.EndDate,
                    Location = updatePropertyDto.Location,
                    UserId = existingProperty.UserId,
                    PropertyTypeId = updatePropertyDto.PropertyTypeId,
                    PropertyStatusId = updatePropertyDto.PropertyStatusId,
                    StartDate = existingProperty.StartDate
                };

                await _serviceManager.PropertyService.UpdateAsync(updatedProperty);
                var propertyDto = await MapToPropertyDto(updatedProperty);

                return Ok(ApiResponse<PropertyDto>.SuccessResult(propertyDto, "Property updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PropertyDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertyDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property sil
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <returns>Silme sonucu</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteProperty(int id)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var existingProperty = await _serviceManager.PropertyService.GetByIdAsync(id);

                // Sadece property sahibi veya admin silebilir
                if (existingProperty.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                await _serviceManager.PropertyService.DeleteAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(null, "Property deleted successfully"));
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
        /// Kullanıcının properties'lerini getir
        /// </summary>
        /// <returns>Kullanıcının property listesi</returns>
        [HttpGet("my-properties")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PropertyDto>>>> GetMyProperties()
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var allProperties = await _serviceManager.PropertyService.GetAllAsync();
                var userProperties = allProperties.Where(p => p.UserId == currentUserId);

                var propertyDtos = await MapToPropertyDtos(userProperties);

                return Ok(ApiResponse<IEnumerable<PropertyDto>>.SuccessResult(propertyDtos, "User properties retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<PropertyDto>>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property özet bilgileri (dashboard için)
        /// </summary>
        /// <returns>Özet bilgiler</returns>
        [HttpGet("summary")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<PropertySummaryDto>>> GetPropertySummary()
        {
            try
            {
                var allProperties = await _serviceManager.PropertyService.GetAllAsync();
                var activeCount = await _serviceManager.PropertyService.GetActivePropertyCountAsync();
                var totalRevenue = await _serviceManager.PropertyService.CalculateTotalRevenueAsync();
                var availableProperties = await _serviceManager.PropertyService.GetAvailablePropertiesAsync(DateTime.Now, DateTime.Now.AddMonths(1));

                var summary = new PropertySummaryDto
                {
                    TotalProperties = allProperties.Count(),
                    ActiveProperties = activeCount,
                    TotalRevenue = totalRevenue,
                    AvailableProperties = availableProperties.Count()
                };

                return Ok(ApiResponse<PropertySummaryDto>.SuccessResult(summary, "Property summary retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertySummaryDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property availability kontrolü
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <param name="startDate">Başlangıç tarihi</param>
        /// <param name="endDate">Bitiş tarihi</param>
        /// <returns>Müsaitlik durumu</returns>
        [HttpGet("{id}/availability")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> CheckPropertyAvailability(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var isAvailable = await _serviceManager.PropertyService.IsPropertyAvailableAsync(id, startDate, endDate);
                return Ok(ApiResponse<bool>.SuccessResult(isAvailable, "Property availability checked"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        #region Private Helper Methods

        private async Task<PropertyDto> MapToPropertyDto(Property property)
        {
            var user = await _serviceManager.UserService.GetUserByIdAsync(property.UserId);
            var propertyType = await _serviceManager.PropertyService.PropertyTypeService.GetTypeIdAsync(property.PropertyTypeId);
            var propertyStatus = await _serviceManager.PropertyService.PropertyStatusService.GetBYIdAsync(property.PropertyStatusId);

            return new PropertyDto
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                Price = property.Price,
                Currency = property.Currency,
                StartDate = property.StartDate,
                EndDate = property.EndDate,
                Location = property.Location,
                UserId = property.UserId,
                UserName = user.UserName,
                PropertyTypeId = property.PropertyTypeId,
                PropertyTypeName = propertyType.Name,
                PropertyStatusId = property.PropertyStatusId,
                PropertyStatusName = propertyStatus.Name,
                Photos = property.Photos?.Select(p => new PropertyPhotoDto
                {
                    Id = p.Id,
                    Url = p.Url,
                    IsMain = p.IsMain.Equals("true", StringComparison.OrdinalIgnoreCase),
                    PropertyId = p.PropertyId
                }).ToList() ?? new List<PropertyPhotoDto>()
            };
        }

        private async Task<List<PropertyDto>> MapToPropertyDtos(IEnumerable<Property> properties)
        {
            var propertyDtos = new List<PropertyDto>();

            foreach (var property in properties)
            {
                var propertyDto = await MapToPropertyDto(property);
                propertyDtos.Add(propertyDto);
            }

            return propertyDtos;
        }

        #endregion
    }
}
