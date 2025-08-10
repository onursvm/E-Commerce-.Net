namespace E_Commerce.Presentation.Dtos.Property
{
    public class PropertySummaryDto
    {
        public int TotalProperties { get; set; }
        public int ActiveProperties { get; set; }
        public decimal TotalRevenue { get; set; }
        public int AvailableProperties { get; set; }
    }
}
