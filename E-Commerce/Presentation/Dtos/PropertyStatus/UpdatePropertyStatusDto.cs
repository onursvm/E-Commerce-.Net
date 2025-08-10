using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Presentation.Dtos.PropertyStatus
{
    public class UpdatePropertyStatusDto
    {
        [Required(ErrorMessage = "Status name is required")]
        [MinLength(2, ErrorMessage = "Status name must be at least 2 characters")]
        public string Name { get; set; }
    }
}
