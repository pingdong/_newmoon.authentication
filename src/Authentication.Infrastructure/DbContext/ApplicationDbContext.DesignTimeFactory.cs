using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PingDong.Newmoon.Authentication.Infrastructure
{
    public class ApplicationDbContextDesignFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Configuration
            var settingFile = Path.Combine(Directory.GetCurrentDirectory(), "local.settings.json");
                
            var config = new ConfigurationBuilder()
                .AddJsonFile(settingFile)
                .Build();
            var connectionString = config.GetConnectionString("Default");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
