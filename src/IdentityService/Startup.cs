using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PingDong.Newmoon.IdentityService.Infrastructure;
using PingDong.Newmoon.IdentityService.Service;
using PingDong.NewMoon.UserManagement;
using PingDong.NewMoon.UserManagement.Infrastructure;
using System;

namespace PingDong.Newmoon.Authentication.IdentityService
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration config)
        {
            _configuration = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region DevOps
            
            // Telemetry (Application Insights)
            services.AddApplicationInsightsTelemetry(_configuration);

            // HealthChecks
            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();

            #endregion

            #region ASP.Net

            #region Asp.Net Identity

            services.AddDbContext<ApplicationDbContext>(context =>
                                        context.UseSqlServer(_configuration.GetConnectionString("Default"),
                                            sqlServerOptionsAction: options =>
                                            {
                                                options.EnableRetryOnFailure(maxRetryCount: 10,
                                                                             maxRetryDelay: TimeSpan.FromSeconds(30),
                                                                         errorNumbersToAdd: null);
                                            }
                                        ));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                    }
                )
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            #endregion

            services.Configure<CookiePolicyOptions>(options =>
                            {
                                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                                options.CheckConsentNeeded = context => true;
                                options.MinimumSameSitePolicy = SameSiteMode.None;
                            });

            services.AddMvc(option => option.EnableEndpointRouting = false)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #endregion

            #region Identity Server 4

            // It’s important when using ASP.NET Identity that IdentityServer be registered after
            // ASP.NET Identity in the DI system because
            // IdentityServer overwrites some configuration from ASP.NET Identity.

            services.AddTransient<IProfileService, ProfileService>();

            services.AddIdentityServer(options => {
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseSuccessEvents = true;
                    })
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddProfileService<ProfileService>()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
                    .AddInMemoryClients(IdentityServerConfig.GetClients(_configuration));

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCookiePolicy();
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
