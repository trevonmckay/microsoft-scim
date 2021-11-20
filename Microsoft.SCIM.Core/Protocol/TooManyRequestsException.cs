using System;

namespace Microsoft.SCIM
{
    public class TooManyRequestsException : Exception
    {
        public TooManyRequestsException(Exception innerExpection = null)
            : base("The request has exceeded the throttle limit. Please try again later", innerExpection)
        {

        }

        public TooManyRequestsException(DateTime retryAfter, Exception innerExpection = null)
            : base($"The request has exceeded the throttle limit. Please try again after {retryAfter}", innerExpection)
        {
            RetryAfter = retryAfter;
        }

        public TooManyRequestsException(TimeSpan retryAfter, Exception innerExpection = null)
            : this(DateTime.UtcNow.Add(retryAfter), innerExpection) { }

        public DateTime RetryAfter { get; }
    }
}
