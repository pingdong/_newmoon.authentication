using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PingDong.Newmoon.IdentityService.Infrastructure
{
    public abstract class IdentityDesignTimeDbContextFactoryBase<TContext> : 
        IDesignTimeDbContextFactory<TContext> where TContext : DbContext
    {
        public TContext CreateDbContext(string[] args)
        {
            // Configuration
            var settingFile = Path.Combine(Directory.GetCurrentDirectory(), "local.settings.json");
                
            var config = new ConfigurationBuilder()
                .AddJsonFile(settingFile, optional: true, reloadOnChange: true)
                .Build();
            var connectionString =  config.GetConnectionString("Default");
            
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return CreateNewInstance(optionsBuilder.Options);
        }

        protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);
    }
}
