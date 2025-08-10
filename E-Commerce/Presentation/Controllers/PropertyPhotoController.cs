using E_Commerce.Business.Manager;
using E_Commerce.DataAccses.Entities.Identity;
using E_Commerce.Presentation.Dtos.Auth;
using E_Commerce.Presentation.Dtos.PropertyPhoto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyPhotoController: ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public PropertyPhotoController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Property'e ait tüm fotoğrafları getir
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <returns>Fotoğraf listesi</returns>
        [HttpGet("property/{propertyId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<PropertyPhotoDto>>>> GetPhotosByProperty(int propertyId)
        {
            try
            {
                var photos = await _serviceManager.PropertyService.PhotoService.GetPhotosBypPropertyAsync(propertyId);
                var photoDtos = photos.Select(p => new PropertyPhotoDto
                {
                    Id = p.Id,
                    Url = p.Url,
                    IsMain = p.IsMain.Equals("true", StringComparison.OrdinalIgnoreCase),
                    PropertyId = p.PropertyId
                }).ToList();

                return Ok(ApiResponse<IEnumerable<PropertyPhotoDto>>.SuccessResult(photoDtos, "Property photos retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<PropertyPhotoDto>>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// ID ile fotoğraf getir
        /// </summary>
        /// <param name="id">Fotoğraf ID</param>
        /// <returns>Fotoğraf bilgileri</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PropertyPhotoDto>>> GetPhotoById(int id)
        {
            try
            {
                var photo = await _serviceManager.PropertyService.PhotoService.GetPhotoIdAsync(id);
                var photoDto = new PropertyPhotoDto
                {
                    Id = photo.Id,
                    Url = photo.Url,
                    IsMain = photo.IsMain.Equals("true", StringComparison.OrdinalIgnoreCase),
                    PropertyId = photo.PropertyId
                };

                return Ok(ApiResponse<PropertyPhotoDto>.SuccessResult(photoDto, "Property photo retrieved successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PropertyPhotoDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertyPhotoDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property'e fotoğraf ekle
        /// </summary>
        /// <param name="addPhotoDto">Fotoğraf bilgileri</param>
        /// <returns>Eklenen fotoğraf</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PropertyPhotoDto>>> AddPropertyPhoto([FromBody] AddPropertyPhotoDto addPhotoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<PropertyPhotoDto>.ErrorResult("Validation failed", errors));
                }

                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Property'nin sahibi mi kontrol et
                var property = await _serviceManager.PropertyService.GetByIdAsync(addPhotoDto.PropertyId);
                if (property.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var addedPhoto = await _serviceManager.PropertyService.PhotoService.AddPhotoAsync(
                    addPhotoDto.PropertyId,
                    addPhotoDto.ImageUrl,
                    addPhotoDto.IsMain);

                var photoDto = new PropertyPhotoDto
                {
                    Id = addedPhoto.Id,
                    Url = addedPhoto.Url,
                    IsMain = addedPhoto.IsMain.Equals("true", StringComparison.OrdinalIgnoreCase),
                    PropertyId = addedPhoto.PropertyId
                };

                return CreatedAtAction(nameof(GetPhotoById), new { id = addedPhoto.Id },
                    ApiResponse<PropertyPhotoDto>.SuccessResult(photoDto, "Property photo added successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<PropertyPhotoDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PropertyPhotoDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Fotoğrafı sil
        /// </summary>
        /// <param name="id">Fotoğraf ID</param>
        /// <returns>Silme sonucu</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeletePropertyPhoto(int id)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Fotoğrafın sahibi mi kontrol et
                var isOwner = await _serviceManager.PropertyService.PhotoService.IsPhotoOwnedByUserAsync(id, currentUserId);
                if (!isOwner && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                await _serviceManager.PropertyService.PhotoService.DeletePhotoAsync(id);
                return Ok(ApiResponse<object>.SuccessResult(null, "Property photo deleted successfully"));
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
        /// Ana fotoğrafı belirle
        /// </summary>
        /// <param name="setMainPhotoDto">Ana fotoğraf bilgileri</param>
        /// <returns>İşlem sonucu</returns>
        [HttpPut("set-main")]
        public async Task<ActionResult<ApiResponse<object>>> SetMainPhoto([FromBody] SetMainPhotoDto setMainPhotoDto)
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

                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Fotoğrafın sahibi mi kontrol et
                var isOwner = await _serviceManager.PropertyService.PhotoService.IsPhotoOwnedByUserAsync(setMainPhotoDto.PhotoId, currentUserId);
                if (!isOwner && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                await _serviceManager.PropertyService.PhotoService.SetMainPhotoAsync(setMainPhotoDto.PropertyId, setMainPhotoDto.PhotoId);
                return Ok(ApiResponse<object>.SuccessResult(null, "Main photo set successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property için fotoğraf yükleme URL'i oluştur
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <returns>Yükleme URL'i</returns>
        [HttpPost("generate-upload-url/{propertyId}")]
        public async Task<ActionResult<ApiResponse<UploadPhotoResponseDto>>> GenerateUploadUrl(int propertyId)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Property'nin sahibi mi kontrol et
                var property = await _serviceManager.PropertyService.GetByIdAsync(propertyId);
                if (property.UserId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var uploadUrl = await _serviceManager.PropertyService.PhotoService.GenerateUploadUrlAsync(propertyId);
                var response = new UploadPhotoResponseDto
                {
                    UploadUrl = uploadUrl,
                    Message = "Upload URL generated successfully"
                };

                return Ok(ApiResponse<UploadPhotoResponseDto>.SuccessResult(response, "Upload URL generated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UploadPhotoResponseDto>.ErrorResult(ex.Message));
            }
            catch (NotImplementedException ex)
            {
                return StatusCode(501, ApiResponse<UploadPhotoResponseDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UploadPhotoResponseDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Property'nin fotoğraf sayısını getir
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <returns>Fotoğraf sayısı</returns>
        [HttpGet("count/{propertyId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<int>>> GetPhotoCountForProperty(int propertyId)
        {
            try
            {
                var count = await _serviceManager.PropertyService.PhotoService.GetPhotoCountForPopertyAsync(propertyId);
                return Ok(ApiResponse<int>.SuccessResult(count, "Photo count retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Fotoğrafın kullanıcıya ait olup olmadığını kontrol et
        /// </summary>
        /// <param name="photoId">Fotoğraf ID</param>
        /// <returns>Sahiplik durumu</returns>
        [HttpGet("{photoId}/ownership")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckPhotoOwnership(int photoId)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var isOwner = await _serviceManager.PropertyService.PhotoService.IsPhotoOwnedByUserAsync(photoId, currentUserId);

                return Ok(ApiResponse<bool>.SuccessResult(isOwner, "Photo ownership checked"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }
    }
}
