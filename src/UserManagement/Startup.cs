using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;

using PingDong.NewMoon.UserManagement.Infrastructure;

namespace PingDong.NewMoon.UserManagement
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
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    // After extending IdentityUser, the below method has to be called to use default logon/register UI
                    .AddDefaultUI(UIFramework.Bootstrap4);
            
            #endregion

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .AddRazorPagesOptions(o => o.Conventions.AddAreaFolderRouteModelConvention("Identity", "/Account/",
                    model =>
                    {
                        foreach (var selector in model.Selectors)
                        {
                            var attributeRouteModel = selector.AttributeRouteModel;
                            attributeRouteModel.Order = -1;
                            attributeRouteModel.Template = attributeRouteModel.Template.Remove(0, "Identity".Length);
                        }
                    }))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
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
                app.UseExceptionHandler("/Error");

                // Using https
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
