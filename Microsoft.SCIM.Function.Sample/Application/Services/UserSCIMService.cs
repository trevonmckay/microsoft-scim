using System;

namespace Microsoft.SCIM.Function.Application.Services
{
    public class UserSCIMService : SCIMService<Core2EnterpriseUser>
    {
        public UserSCIMService(IMonitor monitor, IProvider provider)
            : base(monitor, provider)
        {
        }

        protected override IProviderAdapter<Core2EnterpriseUser> AdaptProvider(IProvider provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Core2EnterpriseUser> result = new Core2EnterpriseUserProviderAdapter(provider);
            return result;
        }
    }
}
