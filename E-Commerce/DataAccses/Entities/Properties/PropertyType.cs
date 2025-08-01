namespace E_Commerce.DataAccses.Entities.Properties
{
    public class PropertyType
    {
        public int Id { get; set; }
        public string Name { get; set; }//"Villa","Arsa","Daire"

        public ICollection<Property> Properties { get; set; }
    }
}
