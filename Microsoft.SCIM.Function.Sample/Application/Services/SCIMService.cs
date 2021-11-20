using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.SCIM.Function.Application.Services
{
    public abstract class SCIMService<T> : ISCIMService<T> where T : Resource
    {
        protected const int defaultTokenExpirationTimeInMins = 120;
        protected const string AttributeValueIdentifier = "{identifier}";
        protected const string HeaderKeyContentType = "Content-Type";
        protected const string HeaderKeyLocation = "Location";

        protected readonly IMonitor _monitor;
        protected readonly IProvider _provider;

        protected SCIMService(IMonitor monitor, IProvider provider)
        {
            _monitor = monitor;
            _provider = provider;
        }

        protected abstract IProviderAdapter<T> AdaptProvider(IProvider provider);

        // Implemented
        public async Task<ActionResult<QueryResponseBase>> GetAsync(HttpRequestMessage Request)
        {
            var correlationIdentifier = string.Empty;
            try
            {
                if (!Request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                IResourceQuery resourceQuery = new ResourceQuery(Request.RequestUri);
                IProviderAdapter<T> provider = this.AdaptProvider(this._provider);
                QueryResponseBase result =
                    await provider
                            .Query(
                                Request,
                                resourceQuery.Filters,
                                resourceQuery.Attributes,
                                resourceQuery.ExcludedAttributes,
                                resourceQuery.PaginationParameters,
                                correlationIdentifier)
                            .ConfigureAwait(false);
                return result;
            }
            catch (ArgumentException argumentException)
            {
                this.HandleException(
                    correlationIdentifier,
                    argumentException,
                    ServiceNotificationIdentifiers.ControllerTemplateQueryArgumentException);
                throw;
            }
            catch (NotImplementedException notImplementedException)
            {
                this.HandleException(
                    correlationIdentifier,
                    notImplementedException,
                    ServiceNotificationIdentifiers.ControllerTemplateQueryNotImplementedException);

                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                this.HandleException(
                    correlationIdentifier,
                    notSupportedException,
                    ServiceNotificationIdentifiers.ControllerTemplateQueryNotSupportedException);

                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (HttpResponseException responseException)
            {
                if (responseException.Response?.StatusCode != HttpStatusCode.NotFound)
                {
                    this.HandleException(
                   correlationIdentifier,
                   responseException.InnerException ?? responseException,
                   ServiceNotificationIdentifiers.ControllerTemplateGetException);
                }

                throw;
            }
            catch (Exception exception)
            {
                this.HandleException(
                   correlationIdentifier,
                   exception,
                   ServiceNotificationIdentifiers.ControllerTemplateQueryException);

                throw;
            }
        }

        public async Task<ActionResult<QueryResponseBase>> GetAsync(HttpRequestMessage request, string identifier)
        {
            string correlationIdentifier = null;
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return new BadRequestResult();
                }


                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                IResourceQuery resourceQuery = new ResourceQuery(request.RequestUri);
                IProviderAdapter<T> provider = this.AdaptProvider(this._provider);

                if (resourceQuery.Filters.Any())
                {
                    if (resourceQuery.Filters.Count != 1)
                    {
                        return new BadRequestResult();
                    }

                    IFilter filter = new Filter(AttributeNames.Identifier, ComparisonOperator.Equals, identifier);
                    filter.AdditionalFilter = resourceQuery.Filters.Single();
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
                        return new NotFoundResult();
                    }

                    Resource result = queryResponse.Resources.Single();


                    return new OkObjectResult(new { message = "200 OK", resource = result });
                }
                else
                {
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
                        return new NotFoundResult();
                    }

                    return new OkObjectResult(new { message = "200 OK", resource = result });
                }
            }
            catch (ArgumentException argumentException)
            {
                HandleException(correlationIdentifier,
                           argumentException,
                           ServiceNotificationIdentifiers.ControllerTemplateGetArgumentException);


                return new BadRequestResult();
            }
            catch (NotImplementedException notImplementedException)
            {
                HandleException(correlationIdentifier,
                            notImplementedException,
                            ServiceNotificationIdentifiers.ControllerTemplateGetNotImplementedException);


                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {

                HandleException(correlationIdentifier,
                            notSupportedException,
                            ServiceNotificationIdentifiers.ControllerTemplateGetNotSupportedException);

                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (HttpResponseException responseException)
            {
                if (responseException.Response?.StatusCode != HttpStatusCode.NotFound)
                {
                    HandleException(correlationIdentifier,
                             responseException.InnerException ?? responseException,
                             ServiceNotificationIdentifiers.ControllerTemplateGetException);
                }

                if (responseException.Response?.StatusCode == HttpStatusCode.NotFound)
                {
                    return new NotFoundResult();
                }

                throw;
            }
            catch (Exception exception)
            {
                HandleException(correlationIdentifier,
                              exception,
                              ServiceNotificationIdentifiers.ControllerTemplateGetException);



                throw;
            }


        }

        // Implemented
        public async Task<IActionResult> PostAsync(HttpRequestMessage request, T resource)
        {
            string correlationIdentifier = string.Empty;
            try
            {
                if (null == resource)
                {
                    return new BadRequestResult();
                }

                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                IProviderAdapter<T> provider = this.AdaptProvider(this._provider);
                var result = await provider.Create(request, resource, correlationIdentifier)
                                                .ConfigureAwait(false);


                return new OkObjectResult(result);
            }
            catch (ArgumentException argumentException)
            {
                HandleException(correlationIdentifier,
                             argumentException,
                             ServiceNotificationIdentifiers.ControllerTemplatePostArgumentException);

                return new BadRequestResult();
            }
            catch (NotImplementedException notImplementedException)
            {
                HandleException(correlationIdentifier,
                              notImplementedException,
                              ServiceNotificationIdentifiers.ControllerTemplatePostNotImplementedException);


                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                HandleException(correlationIdentifier,
                              notSupportedException,
                              ServiceNotificationIdentifiers.ControllerTemplatePostNotSupportedException);

                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (HttpResponseException httpResponseException)
            {

                HandleException(correlationIdentifier,
                               httpResponseException,
                               ServiceNotificationIdentifiers.ControllerTemplatePostNotSupportedException);

                if (httpResponseException.Response.StatusCode == HttpStatusCode.Conflict)
                {
                    return new AspNetCore.Mvc.ConflictResult();
                }
                else
                {
                    return new BadRequestResult();
                }
            }
            catch (Exception exception)
            {
                HandleException(correlationIdentifier,
                                exception,
                                ServiceNotificationIdentifiers.ControllerTemplatePostException);
                throw;
            }
        }

        public Task<IActionResult> PutAsync(HttpRequestMessage request, T patchRequest, string identifier)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<QueryResponseBase>> PatchAsync(HttpRequestMessage request, PatchRequest2 patchRequest, string identifier)
        {
            string correlationIdentifier = null;

            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return new BadRequestResult();
                }

                identifier = Uri.UnescapeDataString(identifier);

                if (null == patchRequest)
                {
                    return new BadRequestResult();
                }


                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                IProviderAdapter<T> provider = this.AdaptProvider(this._provider);
                await provider.Update(request, identifier, patchRequest, correlationIdentifier).ConfigureAwait(false);

                // If EnterpriseUser, return HTTP code 200 and user object, otherwise HTTP code 204
                if (provider.SchemaIdentifier == SchemaIdentifiers.Core2EnterpriseUser)
                {
                    return await this.GetAsync(request, identifier);
                }
                else
                {
                    return new NoContentResult();
                }
            }
            catch (ArgumentException argumentException)
            {
                this.HandleException(
                       correlationIdentifier,
                       argumentException,
                       ServiceNotificationIdentifiers.ControllerTemplatePatchArgumentException);


                return new BadRequestResult();
            }
            catch (NotImplementedException notImplementedException)
            {

                this.HandleException(
                    correlationIdentifier,
                    notImplementedException,
                    ServiceNotificationIdentifiers.ControllerTemplatePatchNotImplementedException);


                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                this.HandleException(
                  correlationIdentifier,
                  notSupportedException,
                  ServiceNotificationIdentifiers.ControllerTemplatePatchNotSupportedException);

                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (HttpResponseException responseException)
            {
                if (responseException.Response?.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                else
                {
                    this.HandleException(
                       correlationIdentifier,
                       responseException.InnerException ?? responseException,
                       ServiceNotificationIdentifiers.ControllerTemplatePatchException);
                }

                throw;
            }
            catch (Exception exception)
            {
                this.HandleException(
                   correlationIdentifier,
                   exception,
                   ServiceNotificationIdentifiers.ControllerTemplatePatchException);

                throw;
            }
        }

        public async Task<IActionResult> Delete(HttpRequestMessage request, string identifier, string correlationIdentifier = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return new BadRequestResult();
                }

                identifier = Uri.UnescapeDataString(identifier);

                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                IProviderAdapter<T> provider = this.AdaptProvider(this._provider);
                await provider.Delete(request, identifier, correlationIdentifier).ConfigureAwait(false);
                return new NoContentResult();
            }
            catch (ArgumentException argumentException)
            {
                HandleException(correlationIdentifier,
                               argumentException,
                               ServiceNotificationIdentifiers.ControllerTemplateDeleteArgumentException);


                return new BadRequestResult();
            }
            catch (HttpResponseException responseException)
            {
                if (responseException.Response?.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                throw;
            }
            catch (NotImplementedException notImplementedException)
            {
                HandleException(correlationIdentifier,
                               notImplementedException,
                               ServiceNotificationIdentifiers.ControllerTemplateDeleteNotImplementedException);

                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                HandleException(correlationIdentifier,
                              notSupportedException,
                              ServiceNotificationIdentifiers.ControllerTemplateDeleteNotSupportedException);

                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (Exception exception)
            {
                HandleException(correlationIdentifier,
                              exception,
                              ServiceNotificationIdentifiers.ControllerTemplateDeleteException);
                throw;
            }
        }

        private void HandleException(string correlationIdentifier, Exception exception, long notificationIdentifier)
        {
            IExceptionNotification notification =
                                ExceptionNotificationFactory.Instance.CreateNotification(
                                    exception,
                                    correlationIdentifier,
                                    notificationIdentifier);

            _monitor.Report(notification);
        }

    }
}
