using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Presentation.Dtos.Role
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "Role name is required")]
        [MinLength(2, ErrorMessage = "Role name must be at least 2 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MinLength(5, ErrorMessage = "Description must be at least 5 characters")]
        public string Description { get; set; }
    }
}
