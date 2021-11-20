using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.SCIM.Function.Infrastructure.Common
{
    /// <summary>
    /// Creates the config of additional libraries ex. MediatR, Automapper
    /// <example>For example:
    /// <code>       
    ///    builder.AddEventSettingsToConfiguration(config);
    /// </code>
    /// </summary>
    public static class EventSettings
    {
        public static IFunctionsHostBuilder AddEventSettingsToConfiguration(this IFunctionsHostBuilder builder, IConfiguration config = null)
        {

            // Registering MediatR provider
            builder.Services
                   .AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

            return builder;
        }
    }
}
