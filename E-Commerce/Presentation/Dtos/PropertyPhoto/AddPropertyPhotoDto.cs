using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Presentation.Dtos.PropertyPhoto
{
    public class AddPropertyPhotoDto
    {
        [Required(ErrorMessage = "Property ID is required")]
        public int PropertyId { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string ImageUrl { get; set; }

        public bool IsMain { get; set; } = false;
    }
}
