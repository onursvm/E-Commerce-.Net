using E_Commerce.Business.Manager;
using E_Commerce.DataAccses.Entities.Identity;
using E_Commerce.Presentation.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;

namespace E_Commerce.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IWebHostEnvironment _env;

        public AuthController(IServiceManager serviceManager, IWebHostEnvironment env)
        {
            _serviceManager = serviceManager;
            _env = env;
        }

        /// <summary>
        /// Kullanıcı girişi
        /// </summary>
        /// <param name="loginDto">Giriş bilgileri</param>
        /// <returns>JWT token ve kullanıcı bilgileri</returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<AuthResponseDto>.ErrorResult("Validation failed", errors));
                }

                // Kullanıcı doğrulama
                var user = await _serviceManager.AuthService.LoginAsync(loginDto.Email, loginDto.Password);

                // JWT Token oluştur
                var accessToken = await _serviceManager.AuthService.GenerateJwtToken(user);
                var refreshToken = await _serviceManager.AuthService.GenerateRefreshTokenAsync(user.Id);

                // Kullanıcı rolleri al
                var userRoles = await _serviceManager.RoleService.GetRolesForUserAsync(user.Id);

                var response = new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    Token = new TokenDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(30), // JWT ayarlarına göre
                        TokenType = "Bearer"
                    },
                    User = new UserInfoDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FullName = user.FullName,
                        IsActive = user.IsActive,
                        Roles = userRoles.Select(r => r.Name).ToList()
                    }
                };

                return Ok(ApiResponse<AuthResponseDto>.SuccessResult(response, "Login successful"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }
        /// <summary>
        /// Kullanıcı kaydı
        /// </summary>
        /// <param name="registerDto">Kayıt bilgileri</param>
        /// <returns>Kayıt sonucu</returns>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<AuthResponseDto>.ErrorResult("Validation failed", errors));
                }

                // Email kontrolü
                if (await _serviceManager.UserService.EmailExistsAsync(registerDto.Email))
                {
                    return BadRequest(ApiResponse<AuthResponseDto>.ErrorResult("Email already exists"));
                }

                // Kullanıcı oluştur
                var newUser = new User
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    FullName = registerDto.FullName,
                    IsActive = true
                };

                var createdUser = await _serviceManager.UserService.CreateUserAsync(newUser, registerDto.Password);

                // Varsayılan "User" rolü ata
                var userRole = await _serviceManager.RoleService.GetAllRoleAsync();
                var defaultRole = userRole.FirstOrDefault(r => r.Name == "User");
                if (defaultRole != null)
                {
                    await _serviceManager.RoleService.AssignRoleToUserAsync(createdUser.Id, defaultRole.Id);
                }

                // JWT Token oluştur
                var accessToken = await _serviceManager.AuthService.GenerateJwtToken(createdUser);
                var refreshToken = await _serviceManager.AuthService.GenerateRefreshTokenAsync(createdUser.Id);

                var response = new AuthResponseDto
                {
                    Success = true,
                    Message = "Registration successful",
                    Token = new TokenDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                        TokenType = "Bearer"
                    },
                    User = new UserInfoDto
                    {
                        Id = createdUser.Id,
                        UserName = createdUser.UserName,
                        Email = createdUser.Email,
                        FullName = createdUser.FullName,
                        IsActive = createdUser.IsActive,
                        Roles = new List<string> { "User" }
                    }
                };

                return CreatedAtAction(nameof(Register), ApiResponse<AuthResponseDto>.SuccessResult(response, "User registered successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }
        /// <summary>
        /// Token yenileme
        /// </summary>
        /// <param name="refreshTokenDto">Refresh token bilgileri</param>
        /// <returns>Yeni token</returns>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<TokenDto>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<TokenDto>.ErrorResult("Validation failed", errors));
                }

                var user = await _serviceManager.AuthService.RefreshTokenAsync(refreshTokenDto.AccessToken, refreshTokenDto.RefreshToken);
                var newAccessToken = await _serviceManager.AuthService.GenerateJwtToken(user);

                var response = new TokenDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = user.RefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                    TokenType = "Bearer"
                };

                return Ok(ApiResponse<TokenDto>.SuccessResult(response, "Token refreshed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TokenDto>.ErrorResult(ex.Message));
            }
        }
        /// <summary>
        /// Çıkış işlemi
        /// </summary>
        /// <returns>Çıkış sonucu</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"]
                    .FirstOrDefault()?.Split(" ").Last();

                if (token != null)
                {
                    await _serviceManager.AuthService.RevokeTokenAsync(token);
                }

                return Ok(ApiResponse<object>.SuccessResult(null, "Logout successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }
        /// <summary>
        /// Şifre değiştirme
        /// </summary>
        /// <param name="changePasswordDto">Şifre değiştirme bilgileri</param>
        /// <returns>Değiştirme sonucu</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
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

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _serviceManager.AuthService.ChangePasswordAsync(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (result)
                {
                    return Ok(ApiResponse<object>.SuccessResult(null, "Password changed successfully"));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to change password"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }
        /// <summary>
        /// Şifre sıfırlama talebi
        /// </summary>
        /// <param name="forgotPasswordDto">Email bilgisi</param>
        /// <returns>Talep sonucu</returns>
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
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

                var token = await _serviceManager.AuthService.GeneratePasswordResetTokenAsync(forgotPasswordDto.Email);

                if (token != null)
                {
                    var data = _env != null && _env.EnvironmentName == "Development" ? new { Token = token } : null;
                    return Ok(ApiResponse<object>.SuccessResult(data, "Password reset email sent"));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("User not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }
        /// <summary>
        /// Şifre sıfırlama
        /// </summary>
        /// <param name="resetPasswordDto">Sıfırlama bilgileri</param>
        /// <returns>Sıfırlama sonucu</returns>
        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
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

                var result = await _serviceManager.AuthService.ResetPasswordAsync(resetPasswordDto.Token, resetPasswordDto.NewPassword);

                if (result)
                {
                    return Ok(ApiResponse<object>.SuccessResult(null, "Password reset successfully"));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Invalid or expired token"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }
        /// <summary>
        /// Kullanıcı profil bilgileri
        /// </summary>
        /// <returns>Kullanıcı bilgileri</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserInfoDto>>> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(ApiResponse<UserInfoDto>.ErrorResult("Invalid user token"));
                }

                var user = await _serviceManager.UserService.GetUserByIdAsync(userId);
                var userRoles = await _serviceManager.RoleService.GetRolesForUserAsync(userId);

                var userInfo = new UserInfoDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    Roles = userRoles.Select(r => r.Name).ToList()
                };

                return Ok(ApiResponse<UserInfoDto>.SuccessResult(userInfo, "Profile retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserInfoDto>.ErrorResult("Internal server error", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Kullanıcı profil bilgilerini güncelle
        /// </summary>
        /// <param name="updateProfileDto">Güncelleme bilgileri</param>
        /// <returns>Güncelleme sonucu</returns>
        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
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

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("Invalid user token"));
                }

                await _serviceManager.UserService.UpdateProfileAsync(userId, updateProfileDto);

                return Ok(ApiResponse<object>.SuccessResult(null, "Profile updated successfully"));
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
    }
}
