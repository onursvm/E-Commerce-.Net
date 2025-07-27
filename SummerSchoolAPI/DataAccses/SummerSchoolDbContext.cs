using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace SummerSchoolAPI.DataAccses
{
    public class SummerSchoolDbContext : DbContext
    {
        public SummerSchoolDbContext(DbContextOptions<SummerSchoolDbContext> options) : base (options)
        {
            

        }
        public DbSet<Book> Books { get; set; }

    }
    
    }

