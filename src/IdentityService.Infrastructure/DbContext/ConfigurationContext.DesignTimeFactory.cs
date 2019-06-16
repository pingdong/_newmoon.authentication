using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace PingDong.Newmoon.IdentityService.Infrastructure
{
    public class ConfigurationContextDesignTimeFactory : IdentityDesignTimeDbContextFactoryBase<ConfigurationDbContext>
    {
        protected override ConfigurationDbContext CreateNewInstance(DbContextOptions<ConfigurationDbContext> options)
        {
            var configOption = new ConfigurationStoreOptions { DefaultSchema = IdentityDbConfig.DefaultSchema };

            return new ConfigurationDbContext(options, configOption);
        }
    }
}
