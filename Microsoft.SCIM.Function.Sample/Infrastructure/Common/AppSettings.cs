using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace Microsoft.SCIM.Function.Infrastructure.Common
{
    /// <summary>
    /// Extracts config from *.settings.json files
    /// </summary>
    public static class AppSettings
    {
        public static IFunctionsHostBuilder AddAppSettingsToConfiguration(this IFunctionsHostBuilder builder)
        {
            var context = builder.GetContext();
            string currentDirectory = context.ApplicationRootPath;

            bool isLocal = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
            if (isLocal)
            {
                currentDirectory = Environment.CurrentDirectory;
            }

            var tmpConfig = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            string environmentName = tmpConfig["Environment"];
            if (string.IsNullOrEmpty(environmentName))
            {
                // default to production
                environmentName = "production";
            }

            var configurationBuilder = new ConfigurationBuilder();

            var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));
            if (descriptor?.ImplementationInstance is IConfiguration configRoot)
            {
                configurationBuilder.AddConfiguration(configRoot);
            }

            var configuration = configurationBuilder.SetBasePath(currentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            if (builder.Services.Contains(ServiceDescriptor.Singleton(typeof(IConfiguration))))
            {
                builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));
            }
            else
            {
                builder.Services.AddSingleton<IConfiguration>(configuration);
            }

            return builder;
        }
    }
}
