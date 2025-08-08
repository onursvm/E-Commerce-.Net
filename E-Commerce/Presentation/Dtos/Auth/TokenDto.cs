using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Presentation.Dtos.Auth
{
    public class TokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
    }
}
