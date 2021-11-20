using System;
using System.Net.Http;

namespace Microsoft.SCIM
{
    public sealed class ScimServiceProviderConfigurationService : BaseScimService
    {
        public ScimServiceProviderConfigurationService(IProvider provider, IMonitor monitor)
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

                ServiceConfigurationBase result = provider.Configuration;
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
                            ServiceNotificationIdentifiers.ServiceProviderConfigurationControllerGetArgumentException);
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
                            ServiceNotificationIdentifiers.ServiceProviderConfigurationControllerGetNotImplementedException);
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
                            ServiceNotificationIdentifiers.ServiceProviderConfigurationControllerGetNotSupportedException);
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
                            ServiceNotificationIdentifiers.ServiceProviderConfigurationControllerGetException);
                    monitor.Report(notification);
                }

                throw;
            }
        }
    }
}
