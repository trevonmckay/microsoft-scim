using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.SCIM.Function.Infrastructure.Common
{
    /// <summary>
    /// Creates the configuration of all DI services/provider/ others that are required.
    /// </summary>
    public static class DependencyInjectionSettings
    {
        public static IFunctionsHostBuilder AddRegistrationDIServicesSettingsToConfiguration(this IFunctionsHostBuilder builder, IConfiguration config = null)
        {
            // Add Registration of DI Services
            builder.Services.AddSingleton(typeof(IProvider), typeof(DGSProvider));
            builder.Services.AddSingleton(typeof(IMonitor), typeof(ConsoleMonitor));

            return builder;
        }
    }
}
