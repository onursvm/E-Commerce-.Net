using E_Commerce.DataAccses.Entities.Identity;
using E_Commerce.DataAccses.Entities.Properties;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace E_Commerce.DataAccses.Context
{
    public class CommerceDbContext : DbContext
    {
        public CommerceDbContext(DbContextOptions<CommerceDbContext> options)
            : base(options) { }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<PropertyStatus> PropertyStatus { get; set; }

        public DbSet<PropertyPhoto> PropertyPhotes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Photos)
                .WithOne(p => p.Property)
                .HasForeignKey(p => p.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            SeedData(modelBuilder);

        }
        private void SeedData(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<PropertyType>().HasData(
                new PropertyType { Id = 1, Name = "Daire" },
                new PropertyType { Id = 2, Name = "Villa" },
                new PropertyType { Id = 3, Name = "Arsa" }
                );

            modelBuilder.Entity<PropertyStatus>().HasData(
                new PropertyStatus { Id = 1, Name = "Satılık" },
                new PropertyStatus { Id = 2, Name = "Kiralık" }
                );

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "Sistem Yöneticisi"
                },
                new Role
                {
                    Id = 2,
                    Name = "User",
                    Description = "Standart Kullanıcı"
                }
                );
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    UserName = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),

                    Email = "admin@emlak.com",
                    IsActive = true
                }
                );
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId=1,RoleId=1}
                );
        }
    }
}