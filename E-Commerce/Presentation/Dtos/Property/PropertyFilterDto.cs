namespace E_Commerce.Presentation.Dtos.Property
{
    public class PropertyFilterDto
    {
        public int? PropertyTypeId { get; set; }
        public int? PropertyStatusId { get; set; }
        public string Location { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
