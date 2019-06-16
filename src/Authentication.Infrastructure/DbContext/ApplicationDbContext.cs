using System;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

namespace PingDong.Newmoon.Authentication.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        private const string DefaultSchema = "user";

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<ApplicationUser>().ToTable("Users", schema: DefaultSchema);
            
            builder.Entity<ApplicationRole>().ToTable("Roles", schema: DefaultSchema);

            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", schema: DefaultSchema);
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", schema: DefaultSchema);
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", schema: DefaultSchema);
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", schema: DefaultSchema);
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", schema: DefaultSchema);
        }
    }
}
