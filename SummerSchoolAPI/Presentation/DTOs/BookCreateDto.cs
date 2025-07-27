using System.ComponentModel.DataAnnotations;

namespace SummerSchoolAPI.Presentation.DTO
{
    public class BookCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        public String Title{ get; set; }
        [Range(1800,2100, ErrorMessage = "Publish date must be between 1800 and 2100")]
        public DateTime PublishDate { get; set; }
    }
}
