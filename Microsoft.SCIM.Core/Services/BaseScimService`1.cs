using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM
{
    public abstract class BaseScimService<T> : BaseScimService where T : Resource
    {
        internal BaseScimService(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        protected abstract IProviderAdapter<T> AdaptProvider(IProvider provider);

        protected virtual IProviderAdapter<T> AdaptProvider()
        {
            IProviderAdapter<T> result = this.AdaptProvider(this.provider);
            return result;
        }

        public virtual async Task<HttpResponseMessage> Get(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            string correlationIdentifier = null;
            try
            {
                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    return InternalServerError();
                }

                IResourceQuery resourceQuery = new ResourceQuery(request.RequestUri);
                IProviderAdapter<T> provider = this.AdaptProvider();
                QueryResponseBase result =
                    await provider
                            .Query(
                                request,
                                resourceQuery.Filters,
                                resourceQuery.Attributes,
                                resourceQuery.ExcludedAttributes,
                                resourceQuery.PaginationParameters,
                                correlationIdentifier)
                            .ConfigureAwait(false);
                return this.Ok(result);
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateQueryArgumentException);
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
                            ServiceNotificationIdentifiers.ControllerTemplateQueryNotImplementedException);
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
                            ServiceNotificationIdentifiers.ControllerTemplateQueryNotSupportedException);
                    monitor.Report(notification);
                }

                return NotImplemented();
            }
            catch (ResourceNotFoundException notFoundException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notFoundException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateQueryNotSupportedException);
                    monitor.Report(notification);
                }

                return NotFound();
            }
            catch (TooManyRequestsException tooManyRequestsException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            tooManyRequestsException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateQueryException);
                    monitor.Report(notification);
                }

                return TooManyRequests(tooManyRequestsException.Message);
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception.InnerException ?? exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateQueryException);
                    monitor.Report(notification);
                }

                throw;
            }
        }

        public virtual async Task<HttpResponseMessage> Get(HttpRequestMessage request, string identifier, CancellationToken cancellationToken = default)
        {
            string correlationIdentifier = null;
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return this.BadRequest();
                }

                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    return InternalServerError();
                }

                IResourceQuery resourceQuery = new ResourceQuery(request.RequestUri);
                if (resourceQuery.Filters.Any())
                {
                    if (resourceQuery.Filters.Count != 1)
                    {
                        return this.BadRequest();
                    }

                    IFilter filter = new Filter(AttributeNames.Identifier, ComparisonOperator.Equals, identifier)
                    {
                        AdditionalFilter = resourceQuery.Filters.Single()
                    };

                    IReadOnlyCollection<IFilter> filters =
                        new IFilter[]
                            {
                                filter
                            };
                    IResourceQuery effectiveQuery =
                        new ResourceQuery(
                            filters,
                            resourceQuery.Attributes,
                            resourceQuery.ExcludedAttributes);
                    IProviderAdapter<T> provider = this.AdaptProvider();
                    QueryResponseBase queryResponse =
                        await provider
                            .Query(
                                request,
                                effectiveQuery.Filters,
                                effectiveQuery.Attributes,
                                effectiveQuery.ExcludedAttributes,
                                effectiveQuery.PaginationParameters,
                                correlationIdentifier)
                            .ConfigureAwait(false);
                    if (!queryResponse.Resources.Any())
                    {
                        return this.NotFound();
                    }

                    Resource result = queryResponse.Resources.Single();
                    return this.Ok(result);
                }
                else
                {
                    IProviderAdapter<T> provider = this.AdaptProvider();
                    Resource result =
                        await provider
                            .Retrieve(
                                request,
                                identifier,
                                resourceQuery.Attributes,
                                resourceQuery.ExcludedAttributes,
                                correlationIdentifier)
                            .ConfigureAwait(false);
                    if (null == result)
                    {
                        return this.NotFound();
                    }

                    return this.Ok(result);
                }
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateGetArgumentException);
                    monitor.Report(notification);
                }

                return this.BadRequest();
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateGetNotImplementedException);
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
                            ServiceNotificationIdentifiers.ControllerTemplateGetNotSupportedException);
                    monitor.Report(notification);
                }

                return NotImplemented();
            }
            catch (ResourceNotFoundException notFoundException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notFoundException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateGetException);
                    monitor.Report(notification);
                }

                return NotFound();
            }
            catch (TooManyRequestsException tooManyRequestsException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            tooManyRequestsException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateGetException);
                    monitor.Report(notification);
                }

                return TooManyRequests(tooManyRequestsException.Message);
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateGetException);
                    monitor.Report(notification);
                }

                throw;
            }
        }

        public virtual async Task<HttpResponseMessage> Post(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            string correlationIdentifier = null;

            T resource;
            string requestBody = await request.Content.ReadAsStringAsync();
            try
            {
                resource = JsonConvert.DeserializeObject<T>(requestBody);
            }
            catch (JsonSerializationException serializationException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            serializationException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostException);
                    monitor.Report(notification);
                }

                return BadRequest();
            }

            try
            {
                if (null == resource)
                {
                    return this.BadRequest();
                }

                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    return InternalServerError();
                }

                IProviderAdapter<T> provider = this.AdaptProvider();
                Resource result = await provider.Create(request, resource, correlationIdentifier).ConfigureAwait(false);
                Uri resourceLocation = GetResourceLocationUri(request, result);
                return Created(resourceLocation, result);
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostArgumentException);
                    monitor.Report(notification);
                }

                return this.BadRequest();
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostNotImplementedException);
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
                            ServiceNotificationIdentifiers.ControllerTemplatePostNotSupportedException);
                    monitor.Report(notification);
                }

                return NotImplemented();
            }
            catch (ResourceExistsException conflictException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            conflictException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostNotSupportedException);
                    monitor.Report(notification);
                }

                return Conflict();
            }
            catch (TooManyRequestsException tooManyRequestsException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            tooManyRequestsException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostException);
                    monitor.Report(notification);
                }

                return TooManyRequests(tooManyRequestsException.Message);
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostException);
                    monitor.Report(notification);
                }

                throw;
            }
        }

        public virtual async Task<HttpResponseMessage> Patch(HttpRequestMessage request, string identifier, CancellationToken cancellationToken = default)
        {
            string correlationIdentifier = null;

            PatchRequest2 patchRequest;
            string requestBody = await request.Content.ReadAsStringAsync();
            try
            {
                patchRequest = JsonConvert.DeserializeObject<PatchRequest2>(requestBody);
            }
            catch (JsonSerializationException serializationException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            serializationException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostException);
                    monitor.Report(notification);
                }

                return BadRequest();
            }

            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return this.BadRequest();
                }

                identifier = Uri.UnescapeDataString(identifier);

                if (null == patchRequest)
                {
                    return this.BadRequest();
                }

                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    return InternalServerError();
                }

                IProviderAdapter<T> provider = this.AdaptProvider();
                await provider.Update(request, identifier, patchRequest, correlationIdentifier).ConfigureAwait(false);

                // If EnterpriseUser, return HTTP code 200 and user object, otherwise HTTP code 204
                if (provider.SchemaIdentifier == SchemaIdentifiers.Core2EnterpriseUser)
                {
                    return await this.Get(request, identifier).ConfigureAwait(false);
                }
                else
                    return this.NoContent();
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePatchArgumentException);
                    monitor.Report(notification);
                }

                return this.BadRequest();
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePatchNotImplementedException);
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
                            ServiceNotificationIdentifiers.ControllerTemplatePatchNotSupportedException);
                    monitor.Report(notification);
                }

                return NotImplemented();
            }
            catch (ResourceNotFoundException notFoundException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notFoundException.InnerException ?? notFoundException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePatchException);
                    monitor.Report(notification);
                }

                return NotFound();
            }
            catch (TooManyRequestsException tooManyRequestsException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            tooManyRequestsException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePatchException);
                    monitor.Report(notification);
                }

                return TooManyRequests(tooManyRequestsException.Message);
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePatchException);
                    monitor.Report(notification);
                }

                throw;
            }
        }

        public virtual async Task<HttpResponseMessage> Put(HttpRequestMessage request, string identifier, CancellationToken cancellationToken = default)
        {
            string correlationIdentifier = null;

            T resource;
            string requestBody = await request.Content.ReadAsStringAsync();
            try
            {
                resource = JsonConvert.DeserializeObject<T>(requestBody);
            }
            catch (JsonSerializationException serializationException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            serializationException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostException);
                    monitor.Report(notification);
                }

                return BadRequest();
            }

            try
            {
                if (null == resource)
                {
                    return this.BadRequest();
                }

                if (string.IsNullOrEmpty(identifier))
                {
                    return this.BadRequest();
                }

                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    return InternalServerError();
                }

                IProviderAdapter<T> provider = this.AdaptProvider();
                Resource result = await provider.Replace(request, resource, correlationIdentifier).ConfigureAwait(false);
                Uri resourceLocation = GetResourceLocationUri(request, resource);
                return Created(resourceLocation, result);
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePutArgumentException);
                    monitor.Report(notification);
                }

                return this.BadRequest();
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePutNotImplementedException);
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
                            ServiceNotificationIdentifiers.ControllerTemplatePutNotSupportedException);
                    monitor.Report(notification);
                }

                return NotImplemented();
            }
            catch (TooManyRequestsException tooManyRequestsException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            tooManyRequestsException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePutException);
                    monitor.Report(notification);
                }

                return TooManyRequests(tooManyRequestsException.Message);
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePutException);
                    monitor.Report(notification);
                }

                throw;
            }
        }

        public virtual async Task<HttpResponseMessage> Delete(HttpRequestMessage request, string identifier, CancellationToken cancellationToken = default)
        {
            string correlationIdentifier = null;
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return this.BadRequest();
                }

                identifier = Uri.UnescapeDataString(identifier);
                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    return InternalServerError();
                }

                IProviderAdapter<T> provider = this.AdaptProvider();
                await provider.Delete(request, identifier, correlationIdentifier).ConfigureAwait(false);
                return NoContent();
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateDeleteArgumentException);
                    monitor.Report(notification);
                }

                return this.BadRequest();
            }
            catch (ResourceNotFoundException notFoundException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notFoundException.InnerException ?? notFoundException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateDeleteException);
                    monitor.Report(notification);
                }

                return NotFound();
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateDeleteNotImplementedException);
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
                            ServiceNotificationIdentifiers.ControllerTemplateDeleteNotSupportedException);
                    monitor.Report(notification);
                }

                return NotImplemented();
            }
            catch (TooManyRequestsException tooManyRequestsException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            tooManyRequestsException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateDeleteException);
                    monitor.Report(notification);
                }

                return TooManyRequests(tooManyRequestsException.Message);
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateDeleteException);
                    monitor.Report(notification);
                }

                throw;
            }
        }
    }
}
