using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace E_Commerce.DataAccses.Context
{
 
      public class CommerceDbContextFactory : IDesignTimeDbContextFactory<CommerceDbContext>
        {
            public CommerceDbContext CreateDbContext(string[] args)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable
                    ("ASPNETCORE_ENVIRONMENT")}.json",optional:true)
                    .Build();
                
                var builder=new DbContextOptionsBuilder<CommerceDbContext>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                builder.UseSqlServer(connectionString,options=>
                {
                    options.MigrationsAssembly(typeof(CommerceDbContext).Assembly.FullName);
                    options.EnableRetryOnFailure(
                        maxRetryCount : 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
                builder.LogTo(Console.WriteLine, LogLevel.Information);
                return new CommerceDbContext(builder.Options);
            }
            }
    }

