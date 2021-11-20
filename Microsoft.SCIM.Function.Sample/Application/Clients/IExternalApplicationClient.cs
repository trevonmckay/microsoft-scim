using System.Collections.Generic;

namespace Microsoft.SCIM.Sample.Application.Clients
{
    public interface IExternalApplicationClient
    {
        Dictionary<string, Core2EnterpriseUser> Users { get; }
    }
}
