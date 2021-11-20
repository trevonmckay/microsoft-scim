using System;

namespace Microsoft.SCIM
{
    public sealed class ScimGroupsService : BaseScimService<Core2Group>
    {
        public ScimGroupsService(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        protected override IProviderAdapter<Core2Group> AdaptProvider(IProvider provider)
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Core2Group> result = new Core2GroupProviderAdapter(provider);
            return result;
        }
    }
}
