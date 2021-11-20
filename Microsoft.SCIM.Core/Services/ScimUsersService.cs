using System;

namespace Microsoft.SCIM
{
    public class ScimUsersService : BaseScimService<Core2EnterpriseUser>
    {
        public ScimUsersService(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        protected override IProviderAdapter<Core2EnterpriseUser> AdaptProvider(IProvider provider)
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Core2EnterpriseUser> result = new Core2EnterpriseUserProviderAdapter(provider);
            return result;
        }
    }
}
