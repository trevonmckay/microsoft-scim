using System;

namespace Microsoft.SCIM
{
    public sealed class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string identifier = null)
        {

        }
    }
}
