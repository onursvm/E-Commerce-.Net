namespace E_Commerce.DataAccses.Entities.Properties
{
    public class PropertyStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } // "Satılık", "Kiralık"
        public ICollection<Property> Properties { get; set; }
    }
}
