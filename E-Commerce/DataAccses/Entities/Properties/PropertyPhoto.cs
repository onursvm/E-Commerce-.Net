namespace E_Commerce.DataAccses.Entities.Properties
{
    public class PropertyPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string IsMain { get; set; }
        public int PropertyId { get; set; }

        // Navigation property to Property
        public Property Property
        {
            get; set;
        }
    }
}
