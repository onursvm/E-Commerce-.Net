using SummerSchoolAPI.Presentation.DTO;

namespace SummerSchoolAPI.Presentation.DTOs
{
    public class BookResponseDto
    {
        public bool Success { get; set; }
        public String Message { get; set; }
        public BookGetDto Dto { get; set; }
        public List<string>Errors { get; set; }=new List<string>();
    }
}
