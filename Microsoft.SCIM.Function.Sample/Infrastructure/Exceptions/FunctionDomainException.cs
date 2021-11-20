using System;

namespace Microsoft.SCIM.Infrastructure.Exceptions
{
    public class FunctionDomainException : Exception
    {
        public FunctionDomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
