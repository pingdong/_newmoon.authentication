using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace PingDong.NewMoon.UserManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configuration
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                // Application related setting
                                //   for example: log setting
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                // If the same settings in appsettings.json needs to be replaced in development environment, write here
                                //   for example: different logging setting
                                .AddJsonFile($"appsettings.{env}.json", optional: true)
                                .AddEnvironmentVariables()
                                // Environment-aware settings
                                //   for example: external service uri
                                .AddCommandLine(args);

            // In development
            if (string.Equals(env, "development", StringComparison.InvariantCultureIgnoreCase))
            {
                builder
                    // Or in Visual Studio
                    // Some secrets that used in Development could save in User Secrets
                    //   For example: you probably don't want to share you database credential,
                    //                if you want to share your codes.
                    // https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets
                    //     dotnet user-secrets set "Database:DbPassword" "pass123"
                    //     dotnet user-secrets list
                    //     dotnet user-secrets remove "Database:DbPassword"
                    //     dotnet user-secrets clear
                    //
                    // Right click on the target project, then click 'Manage User Secrets'
                    .AddUserSecrets<Startup>(optional: true)
                    .AddJsonFile("local.settings.json", optional: true);
            }

            var configuration = builder.Build();

            // Host
            BuildWebHost(args, configuration)
                .Build()
                .Run();
        }

        public static IHostBuilder BuildWebHost(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddConfiguration(configuration);
                    });
                    builder.UseStartup<Startup>();
                });
    }
}
