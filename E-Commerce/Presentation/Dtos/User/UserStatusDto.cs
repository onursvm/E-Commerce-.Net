using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Presentation.Dtos.User
{
    public class UserStatusDto
    {
        [Required]
        public bool IsActive { get; set; }

    }
}
