using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Presentation.Dtos.User
{
    public class AssignRoleDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Role ID is required")]
        public int RoleId { get; set; }
    
}
}
