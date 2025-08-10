namespace E_Commerce.Presentation.Dtos.PropertyPhoto
{
    public class PropertyPhotoDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public int PropertyId { get; set; }
    }
}
