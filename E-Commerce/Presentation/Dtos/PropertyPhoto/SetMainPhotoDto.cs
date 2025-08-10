using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Presentation.Dtos.PropertyPhoto
{
    public class SetMainPhotoDto
    {
        [Required(ErrorMessage = "Property ID is required")]
        public int PropertyId { get; set; }

        [Required(ErrorMessage = "Photo ID is required")]
        public int PhotoId { get; set; }
    }
}
