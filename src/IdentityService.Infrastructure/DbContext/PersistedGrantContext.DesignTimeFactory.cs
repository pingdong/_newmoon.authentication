using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace PingDong.Newmoon.IdentityService.Infrastructure
{
    public class PersistedGrantContextDesignTimeFactory : IdentityDesignTimeDbContextFactoryBase<PersistedGrantDbContext>
    {
        protected override PersistedGrantDbContext CreateNewInstance(DbContextOptions<PersistedGrantDbContext> options)
        {
            var operationOption = new OperationalStoreOptions { DefaultSchema = IdentityDbConfig.DefaultSchema };

            return new PersistedGrantDbContext(options, operationOption);
        }
    }
}
