using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Presentation.Dtos.Auth
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Access token is required")]
        public string AccessToken { get; set; }

        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }
    }

