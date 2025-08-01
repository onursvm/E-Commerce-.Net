namespace E_Commerce.DataAccses.Entities.Properties
{
    public class Property
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public int Currency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }

        // Navigation Properties
        public int PropertyTypeId { get; set; }
        public PropertyType PropertyType { get; set; }

        public int PropertStatusId { get; set; }
        public PropertyStatus PropertyStatus { get; set; }

        public ICollection<PropertyPhoto> Photos { get; set; } = new List<PropertyPhoto>();
    }
}
