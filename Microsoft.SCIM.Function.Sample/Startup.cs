using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SCIM.Function.Infrastructure.Common;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Microsoft.SCIM.Function.Startup))]
namespace Microsoft.SCIM.Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Add Configuration Settings
            builder.AddAppSettingsToConfiguration();

            // Add Event and Automapper Settings
            builder.AddEventSettingsToConfiguration();

            var _config = builder.Services
                                 .BuildServiceProvider()
                                 .GetService<IConfiguration>();

            // Add Custom Logger Serilog configuration
            builder.AddLoggerSettingsToConfiguration(_config);


            //Add Registration of DI Services
            builder.AddRegistrationDIServicesSettingsToConfiguration(_config);

            //Add Authentication
            ///builder.AddAuthenticationSettingsToConfiguration(_config);

            // Add OpenApi Services
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly());
        }
    }
}
