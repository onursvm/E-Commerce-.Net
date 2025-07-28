using System.ComponentModel.DataAnnotations;

namespace SummerSchoolAPI.DataAccses
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public required String Title { get; set; }

        [Required]
        public DateTime PublishDte { get; set; }
}
}
