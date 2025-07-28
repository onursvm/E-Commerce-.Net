using System.ComponentModel.DataAnnotations;

namespace SummerSchoolAPI.Presentation.DTO
{
    public class UpdateBook
    {
        [Required(ErrorMessage = "ID is required")]
        public int Id{ get; set; }
        public string Title{ get; set; }
        public DateTime PublishDte { get; set; }
    }
}
