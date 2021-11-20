using System;

namespace Microsoft.SCIM.Function.Application.Services
{
    public class GroupSCIMService : SCIMService<Core2Group>
    {
        public GroupSCIMService(IMonitor monitor, IProvider provider)
            : base(monitor, provider)
        {
        }

        protected override IProviderAdapter<Core2Group> AdaptProvider(IProvider provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Core2Group> result = new Core2GroupProviderAdapter(provider);
            return result;
        }
    }
}