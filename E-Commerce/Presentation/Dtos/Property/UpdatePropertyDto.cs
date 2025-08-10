using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Presentation.Dtos.Property
{
    public class UpdatePropertyDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MinLength(5, ErrorMessage = "Title must be at least 5 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        public int Currency { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [MinLength(3, ErrorMessage = "Location must be at least 3 characters")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Property type ID is required")]
        public int PropertyTypeId { get; set; }

        [Required(ErrorMessage = "Property status ID is required")]
        public int PropertyStatusId { get; set; }
    }
}
