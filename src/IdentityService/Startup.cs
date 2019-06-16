using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;

using PingDong.Newmoon.Authentication.Infrastructure;
using PingDong.Newmoon.IdentityService.Service;
using PingDong.Newmoon.IdentityService.Infrastructure;

using System;
using System.Reflection;

namespace PingDong.Newmoon.Authentication.IdentityService
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            _configuration = config;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("Default");

            #region DevOps

            if (_env.IsProduction())
            {
                // Telemetry (Application Insights)
                services.AddApplicationInsightsTelemetry(_configuration);
            }

            // HealthChecks
            services.AddHealthChecks(checks =>
            {
                checks.AddSqlCheck("Database Connectivity", connectionString);
            });

            #endregion
            
            #region ASP.Net

            #region Asp.Net Identity

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions
                            .EnableRetryOnFailure(maxRetryCount: 10,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                    }
                ));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            #endregion

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            #endregion

            #region Identity Server 4

            // It’s important when using ASP.NET Identity that IdentityServer be registered after
            // ASP.NET Identity in the DI system because IdentityServer is overwriting some configuration from ASP.NET Identity.

            services.AddTransient<IProfileService, ProfileService>();

            var identityBuilder = services.AddIdentityServer(options => { })
                                          .AddAspNetIdentity<ApplicationUser>()
                                          .AddProfileService<ProfileService>();

#if DEBUG
            if (_env.IsDevelopment())
            {
                identityBuilder.AddDeveloperSigningCredential()
                               .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                               .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
                               .AddInMemoryClients(IdentityServerConfig.GetClients(_configuration));
            }
#endif
            
            if (_env.IsProduction())
            {
                var migrationsAssembly = typeof(IdentityDbConfig).GetTypeInfo().Assembly.GetName().Name;

                identityBuilder.AddConfigurationStore(options =>
                                    {
                                        options.DefaultSchema = IdentityDbConfig.DefaultSchema;
                                        options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                                            sqlServerOptionsAction: sqlOptions =>
                                            {
                                                sqlOptions.MigrationsAssembly(migrationsAssembly)
                                                    .EnableRetryOnFailure(maxRetryCount: 15,
                                                        maxRetryDelay: TimeSpan.FromSeconds(30),
                                                        errorNumbersToAdd: null);

                                            });
                                    })
                                .AddOperationalStore(options =>
                                    {
                                        options.DefaultSchema = IdentityDbConfig.DefaultSchema;
                                        // this enables automatic token cleanup. this is optional.
                                        options.EnableTokenCleanup = true;
                                        options.TokenCleanupInterval = 30;
                                        options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                                            sqlServerOptionsAction: sqlOptions =>
                                            {
                                                sqlOptions.MigrationsAssembly(migrationsAssembly)
                                                    .EnableRetryOnFailure(maxRetryCount: 15,
                                                        maxRetryDelay: TimeSpan.FromSeconds(30),
                                                        errorNumbersToAdd: null);

                                            });
                                    })
                                .AddSigningCredential(CertificateHelp.Get());
            }
            
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                // Make work identity server redirections in Edge and lastest versions of browers. WARN: Not valid in a production environment.
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
                    await next();
                });
            }
            else
            {
                using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                    var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                    context.Database.Migrate();

                    var seed = new IdentityConfigurationSeed();
                    seed.SeedAsync(context, _configuration).Wait();
                }

                // Using https
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseIdentityServer();

            app.UseMvc();
        }
    }
}
