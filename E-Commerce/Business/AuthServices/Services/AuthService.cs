using E_Commerce.Business.AuthServices.Interfaces;
using E_Commerce.Business.Services.IdentityServices.Interfaces;
using E_Commerce.DataAccses.Entities.Identity;
using E_Commerce.DataAccses.Interfaces.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_Commerce.Business.AuthServices.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepository,
            IRoleService roleService,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _roleService = roleService;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            if (!await _userRepository.CheckPasswordAsync(userId, currentPassword))
                return false;

            await _userRepository.ResetPasswordAsync(userId, newPassword);
            return true;
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]);

            // Kullanıcının rollerini al
            var roles = await GetUserRolesAsync(user.Id);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role.Name)).ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            
            // Rolleri claims'e ekle
            claims.AddRange(roleClaims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:TokenLifetimeMinutes")),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            return await _roleService.GetRolesForUserAsync(userId);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            var token = Guid.NewGuid().ToString();
            var expires = DateTime.UtcNow.AddHours(2);

            await _userRepository.CreatePasswordResetTokenAsync(user.Id, token, expires);
            await _emailService.SendPasswordResetEmailAsync(email, token);

            return token;
        }

        public async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiryTime = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryDays"));

            await _userRepository.UpdateRefreshTokenAsync(userId, refreshToken, expiryTime);
            return refreshToken;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user==null || !user.IsActive)
                throw new UnauthorizedAccessException("Kullanıcı bulunamadı veya aktif değil");

             if(!await _userRepository.CheckPasswordAsync(user.Id, password))
                throw new UnauthorizedAccessException("Geçersiz şifre");

             user.RefreshToken= await GenerateRefreshTokenAsync(user.Id);
            user.PasswordHash = null;

            return user;
        }

        public async Task<User> RefreshTokenAsync(string token, string refreshToken)
        {
            var principal = ValidateJwtToken(token);
            if (principal == null)
                throw new SecurityTokenException("Geçersiz token");

            var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || user.RefreshToken != refreshToken)
                throw new SecurityTokenException("Geçersiz refresh token");

            // Yeni token'lar oluştur
            var newRefreshToken = await GenerateRefreshTokenAsync(user.Id);
            user.RefreshToken = newRefreshToken;
            user.PasswordHash = null;

            return user;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _userRepository.GetByPasswordResetTokenAsync(token);
            if (user == null) return false;

            await _userRepository.ResetPasswordAsync(user.Id, newPassword);
            await _userRepository.CreatePasswordResetTokenAsync(user.Id, null, DateTime.MinValue);

            return true;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var principal = ValidateJwtToken(token);
            if (principal == null) return false;

            var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
            await _userRepository.UpdateRefreshTokenAsync(userId, null, null);
            return true;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await _emailService.SendEmailAsync(to, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            var resetLink = $"{_configuration["ClientApp:Url"]}/reset-password?token={resetToken}";
            var body = $"Şifre sıfırlama linkiniz: <a href='{resetLink}'>Tıklayınız</a>";
            await _emailService.SendEmailAsync(email, "Şifre Sıfırlama Talebi", body);
        }

        public ClaimsPrincipal? ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null &&
                   user.RefreshToken == refreshToken &&
                   user.RefreshTokenExpiryTime > DateTime.UtcNow;
        }
    }
}
