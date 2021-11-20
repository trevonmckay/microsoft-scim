using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class ResourceCommandHandlerBase
    {
        public ResourceCommandHandlerBase(IConfiguration configuration,
                                          IMonitor monitor,
                                          IProvider provider)
        {
            this.configuration1 = configuration;
            this.monitor = monitor;
            this.provider = provider;
        }

        protected const int defaultTokenExpirationTimeInMins = 120;

        protected const string AttributeValueIdentifier = "{identifier}";

        protected const string HeaderKeyContentType = "Content-Type";

        protected const string HeaderKeyLocation = "Location";

        protected readonly IConfiguration configuration;

        protected readonly ILogger _logger;

        protected readonly IMonitor _monitor;

        protected readonly IProvider _provider;

        protected IConfiguration configuration1;

        protected IMonitor monitor;

        protected IProvider provider;
    }
}