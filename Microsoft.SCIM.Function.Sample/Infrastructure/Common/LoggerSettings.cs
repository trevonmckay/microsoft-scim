using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Serilog;
using System;

namespace Microsoft.SCIM.Function.Infrastructure.Common
{
    /// <summary>
    /// Creates the config of the logging library ex. Serilog
    /// <example>For example:
    /// <code>       
    ///    builder.AddLoggerSettingsToConfiguration(config);
    /// </code>
    /// </summary>
    public static class LoggerSettings
    {
        public static IFunctionsHostBuilder AddLoggerSettingsToConfiguration(this IFunctionsHostBuilder builder, IConfiguration config)
        {
            // Registering Serilog provider
            try
            {
                var connectionString = config["AzureWebJobsStorage"];
                var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

                var logger = new LoggerConfiguration()
                                .WriteTo.Console()
                                .WriteTo.File($"log-{ DateTime.UtcNow.ToShortDateString() }.txt", rollingInterval: RollingInterval.Day)
                                .WriteTo.AzureBlobStorage(connectionString)
                                .WriteTo.AzureTableStorage(cloudStorageAccount, storageTableName: "Logs")

                                .CreateLogger();

                builder.Services.AddLogging(lb => lb.AddSerilog(logger));
            }
            catch (Exception)
            {
            }
            return builder;
        }
    }
}
