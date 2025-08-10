using E_Commerce.Business.Manager;
using E_Commerce.Presentation.Dtos.Auth;
using E_Commerce.Presentation.Dtos.PropertyType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyTypeController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public PropertyTypeController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Tüm property tiplerini getir
        /// </summary>
        /// <returns>Property type listesi</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<PropertyTypeDto>>>> GetAllPropertyTypes()
        {
            try
            {
                var propertyTypes = await _serviceManager.PropertyService.PropertyTypeService.GetAllAsync();
                var propertyTypeDtos = new List<PropertyTypeDto>();

                foreach (var type in propertyTypes)
                {
                    var propertyCount = await _serviceManager.PropertyService.PropertyTypeService.GetPropertyCountByTypeAsync(type.Id);
                    propertyTypeDtos.Add(new PropertyTypeDto
                    {
                        Id = type.Id,
                        Name = type.Name,
                        PropertyCount = propertyCount
                    });
                }

                return Ok(ApiResponse<IEnumerable<PropertyTypeDto>>.SuccessResult(propertyTypeDtos, "Property types retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<PropertyTypeDto>>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// ID ile property type getir
        /// </summary>
        /// <param name="id">Property type ID</param>
        /// <returns>Property type bilgileri</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PropertyTypeDto>>> GetPropertyTypeById(int id)
        {
            try
            {
                var propertyType = await _serviceManager.PropertyService.PropertyTypeService.GetTypeIdAsync(id);
                var propertyCount = await _serviceManager.PropertyService.PropertyTypeService.GetPropertyCountByTypeAsync(id);

                var propertyTypeDto = new PropertyTypeDto
                {
                    Id = propertyType.Id,
                    Name = propertyType.Name,
                    PropertyCount = propertyCount
                };

                return Ok(ApiResponse<PropertyTypeDto>.SuccessResult(propertyTypeDto, "Property type retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PropertyTypeDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertyTypeDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Yeni property type oluştur
        /// </summary>
        /// <param name="createPropertyTypeDto">Property type bilgileri</param>
        /// <returns>Oluşturulan property type</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<PropertyTypeDto>>> CreatePropertyType([FromBody] CreatePropertyTypeDto createPropertyTypeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<PropertyTypeDto>.ErrorResult("Validation failed", errors));
                }

                var createdPropertyType = await _serviceManager.PropertyService.PropertyTypeService.CreateAsync(createPropertyTypeDto.Name);
                var propertyTypeDto = new PropertyTypeDto
                {
                    Id = createdPropertyType.Id,
                    Name = createdPropertyType.Name,
                    PropertyCount = 0
                };

                return CreatedAtAction(nameof(GetPropertyTypeById), new { id = createdPropertyType.Id },
                    ApiResponse<PropertyTypeDto>.SuccessResult(propertyTypeDto, "Property type created successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<PropertyTypeDto>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PropertyTypeDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertyTypeDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property type güncelle
        /// </summary>
        /// <param name="id">Property type ID</param>
        /// <param name="updatePropertyTypeDto">Güncellenecek bilgiler</param>
        /// <returns>Güncellenmiş property type</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<PropertyTypeDto>>> UpdatePropertyType(int id, [FromBody] UpdatePropertyTypeDto updatePropertyTypeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<PropertyTypeDto>.ErrorResult("Validation failed", errors));
                }

                await _serviceManager.PropertyService.PropertyTypeService.UpdateAsync(id, updatePropertyTypeDto.Name);
                var updatedPropertyType = await _serviceManager.PropertyService.PropertyTypeService.GetTypeIdAsync(id);
                var propertyCount = await _serviceManager.PropertyService.PropertyTypeService.GetPropertyCountByTypeAsync(id);

                var propertyTypeDto = new PropertyTypeDto
                {
                    Id = updatedPropertyType.Id,
                    Name = updatedPropertyType.Name,
                    PropertyCount = propertyCount
                };

                return Ok(ApiResponse<PropertyTypeDto>.SuccessResult(propertyTypeDto, "Property type updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<PropertyTypeDto>.ErrorResult(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PropertyTypeDto>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PropertyTypeDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertyTypeDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property type sil
        /// </summary>
        /// <param name="id">Property type ID</param>
        /// <returns>Silme sonucu</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> DeletePropertyType(int id)
        {
            try
            {
                await _serviceManager.PropertyService.PropertyTypeService.DeleteAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(null, "Property type deleted successfully"));
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
        /// Property type arama
        /// </summary>
        /// <param name="keyword">Arama kelimesi</param>
        /// <returns>Arama sonuçları</returns>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<PropertyTypeDto>>>> SearchPropertyTypes([FromQuery] string keyword)
        {
            try
            {
                var propertyTypes = await _serviceManager.PropertyService.PropertyTypeService.SearchTypeAsync(keyword);
                var propertyTypeDtos = new List<PropertyTypeDto>();

                foreach (var type in propertyTypes)
                {
                    var propertyCount = await _serviceManager.PropertyService.PropertyTypeService.GetPropertyCountByTypeAsync(type.Id);
                    propertyTypeDtos.Add(new PropertyTypeDto
                    {
                        Id = type.Id,
                        Name = type.Name,
                        PropertyCount = propertyCount
                    });
                }

                return Ok(ApiResponse<IEnumerable<PropertyTypeDto>>.SuccessResult(propertyTypeDtos, "Property types search completed"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<PropertyTypeDto>>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property type kullanım kontrolü
        /// </summary>
        /// <param name="id">Property type ID</param>
        /// <returns>Kullanım durumu</returns>
        [HttpGet("{id}/in-use")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> IsPropertyTypeInUse(int id)
        {
            try
            {
                var inUse = await _serviceManager.PropertyService.PropertyTypeService.IsTypeInUseAsync(id);
                return Ok(ApiResponse<bool>.SuccessResult(inUse, "Property type usage status checked"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property type adı doğrulama
        /// </summary>
        /// <param name="name">Type adı</param>
        /// <returns>Doğrulama sonucu</returns>
        [HttpGet("validate-name")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> ValidatePropertyTypeName([FromQuery] string name)
        {
            try
            {
                var isValid = await _serviceManager.PropertyService.PropertyTypeService.ValidateTypeName(name);
                return Ok(ApiResponse<bool>.SuccessResult(isValid, "Property type name validation completed"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }
    }
}
