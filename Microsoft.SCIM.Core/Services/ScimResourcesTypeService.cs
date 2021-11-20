using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.SCIM
{
    public sealed class ScimResourcesTypeService : BaseScimService
    {
        public ScimResourcesTypeService(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            string correlationIdentifier = null;

            try
            {
                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    return InternalServerError();
                }

                IProvider provider = this.provider;
                if (null == provider)
                {
                    return InternalServerError();
                }

                IEnumerable<Core2ResourceType> result = provider.ResourceTypes;
                return Ok(result);
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ResourceTypesControllerGetArgumentException);
                    monitor.Report(notification);
                }

                return BadRequest();
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ResourceTypesControllerGetNotImplementedException);
                    monitor.Report(notification);
                }

                return NotImplemented();
            }
            catch (NotSupportedException notSupportedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                       ExceptionNotificationFactory.Instance.CreateNotification(
                           notSupportedException,
                           correlationIdentifier,
                           ServiceNotificationIdentifiers.ResourceTypesControllerGetNotSupportedException);
                    monitor.Report(notification);
                }

                return NotImplemented();
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                       ExceptionNotificationFactory.Instance.CreateNotification(
                           exception,
                           correlationIdentifier,
                           ServiceNotificationIdentifiers.ResourceTypesControllerGetException);
                    monitor.Report(notification);
                }

                throw;
            }
        }
    }
}
