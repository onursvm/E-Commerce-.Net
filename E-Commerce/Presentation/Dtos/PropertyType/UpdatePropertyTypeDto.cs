using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Presentation.Dtos.PropertyType
{
    public class UpdatePropertyTypeDto
    {
        [Required(ErrorMessage = "Type name is required")]
        [MinLength(2, ErrorMessage = "Type name must be at least 2 characters")]
        public string Name { get; set; }
    }
}
