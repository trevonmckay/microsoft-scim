namespace Microsoft.SCIM
{
    public class ScimTokenService : BaseScimService
    {
        public ScimTokenService(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }
    }
}
