using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PingDong.NewMoon.UserManagement.Infrastructure;
using System;

namespace PingDong.NewMoon.UserManagement
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
            #region Operation Support
            
            // Telemetry
            services.AddApplicationInsightsTelemetry(_configuration);
            
            // HealthChecks
            services.AddHealthChecks()
                    .AddDbContextCheck<ApplicationDbContext>();

            #endregion

            #region Infrastructure

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("Default"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    }
                ));

            #endregion

            #region ASP.Net

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                                    {
                                        options.Password.RequireDigit = false;
                                        options.Password.RequireNonAlphanumeric = false;
                                        options.Password.RequireUppercase = false;
                                    })
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultUI();

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
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            
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
                app.UseExceptionHandler("/Error");

                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
