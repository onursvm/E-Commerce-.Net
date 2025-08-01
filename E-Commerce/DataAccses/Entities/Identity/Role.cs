namespace E_Commerce.DataAccses.Entities.Identity
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } // "Admin", "User"
        public string Description { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
