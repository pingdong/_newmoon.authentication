using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PingDong.NewMoon.UserManagement.Infrastructure
{
    // This class is used for EF core migration
    public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Configuration
            var configurationFile = Path.Combine(Directory.GetCurrentDirectory(), "local.settings.json");
            var config = new ConfigurationBuilder()
                .AddJsonFile(configurationFile, optional:true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("Default");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
