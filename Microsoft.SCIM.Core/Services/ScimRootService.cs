using System;

namespace Microsoft.SCIM
{
    public sealed class ScimRootService : BaseScimService<Resource>
    {
        public ScimRootService(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        protected override IProviderAdapter<Resource> AdaptProvider(IProvider provider)
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Resource> result = new RootProviderAdapter(provider);
            return result;
        }
    }
}
