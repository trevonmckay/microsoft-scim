using System;

namespace Microsoft.SCIM.Function.Application.Services
{
    public class ResourceSCIMService : SCIMService<Resource>
    {
        public ResourceSCIMService(IMonitor monitor, IProvider provider)
            : base(monitor, provider)
        {
        }

        protected override IProviderAdapter<Resource> AdaptProvider(IProvider provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Resource> result = new RootProviderAdapter(provider);
            return result;
        }
    }
}