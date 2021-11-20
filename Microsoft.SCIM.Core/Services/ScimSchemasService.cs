using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.SCIM
{
    public sealed class ScimSchemasService : BaseScimService
    {
        public ScimSchemasService(IProvider provider, IMonitor monitor)
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

                IEnumerable<TypeScheme> result = provider.Schema;
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
                            ServiceNotificationIdentifiers.SchemasControllerGetArgumentException);
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
                            ServiceNotificationIdentifiers.SchemasControllerGetNotImplementedException);
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
                            ServiceNotificationIdentifiers.SchemasControllerGetNotSupportedException);
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
                            ServiceNotificationIdentifiers.SchemasControllerGetException);
                    monitor.Report(notification);
                }

                throw;
            }
        }
    }
}
