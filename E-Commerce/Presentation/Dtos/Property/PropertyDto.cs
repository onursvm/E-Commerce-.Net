using E_Commerce.Presentation.Dtos.PropertyPhoto;

namespace E_Commerce.Presentation.Dtos.Property
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Currency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int PropertyTypeId { get; set; }
        public string PropertyTypeName { get; set; }
        public int PropertyStatusId { get; set; }
        public string PropertyStatusName { get; set; }
        public List<PropertyPhotoDto> Photos { get; set; } = new List<PropertyPhotoDto>();
    }
}
