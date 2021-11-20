using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class GetUserByIdentifierCommand : MediatR.IRequest<ActionResult<QueryResponseBase>>
    {
        public GetUserByIdentifierCommand(HttpRequestMessage req, string identifier = null)
        {
            this.Request = req;
            this.Identifier = identifier;
        }

        public HttpRequestMessage Request { get; }

        public string Identifier { get; }
    }

    public class GetUserByIdentifierCommandHandler : IRequestHandler<GetUserByIdentifierCommand, ActionResult<QueryResponseBase>>
    {
        protected const int defaultTokenExpirationTimeInMins = 120;
        protected const string AttributeValueIdentifier = "{identifier}";
        protected const string HeaderKeyContentType = "Content-Type";
        protected const string HeaderKeyLocation = "Location";

        protected readonly IConfiguration configuration;
        protected readonly ILogger _logger;
        protected readonly IMonitor _monitor;
        protected readonly IProvider _provider;
        protected IConfiguration configuration1;

        private readonly ILogger<GetUserByIdentifierCommandHandler> logger;
        protected IProviderAdapter<Core2EnterpriseUser> AdaptProvider(IProvider provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Core2EnterpriseUser> result = new Core2EnterpriseUserProviderAdapter(provider);
            return result;
        }

        public GetUserByIdentifierCommandHandler(IConfiguration configuration,
                                       ILogger<GetUserByIdentifierCommandHandler> logger,
                                       IMonitor monitor,
                                       IProvider provider)
        {
            this.logger = logger;
            this.configuration = configuration;
            this._monitor = monitor;
            this._provider = provider;
        }

        public async Task<ActionResult<QueryResponseBase>> Handle(GetUserByIdentifierCommand command, CancellationToken cancellationToken)
        {
            string correlationIdentifier = null;
            try
            {
                if (string.IsNullOrWhiteSpace(command.Identifier))
                {
                    return new BadRequestResult();
                }

                //HttpRequestMessage request = this.ConvertRequest

                if (!command.Request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                IResourceQuery resourceQuery = new ResourceQuery(command.Request.RequestUri);
                IProviderAdapter<Core2EnterpriseUser> provider = this.AdaptProvider(this._provider);

                if (resourceQuery.Filters.Any())
                {
                    if (resourceQuery.Filters.Count != 1)
                    {
                        return new BadRequestResult();
                    }

                    IFilter filter = new Filter(AttributeNames.Identifier, ComparisonOperator.Equals, command.Identifier);
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

                    //IProviderAdapter<T> provider = this.AdaptProvider();


                    QueryResponseBase queryResponse =
                        await provider
                            .Query(
                                command.Request,
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
                                command.Request,
                                command.Identifier,
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

                IExceptionNotification notification =
                    ExceptionNotificationFactory.Instance.CreateNotification(
                        argumentException,
                        correlationIdentifier,
                        ServiceNotificationIdentifiers.ControllerTemplateGetArgumentException);
                _monitor.Report(notification);


                return new BadRequestResult();
            }
            catch (NotImplementedException notImplementedException)
            {
                IExceptionNotification notification =
                    ExceptionNotificationFactory.Instance.CreateNotification(
                        notImplementedException,
                        correlationIdentifier,
                        ServiceNotificationIdentifiers.ControllerTemplateGetNotImplementedException);
                _monitor.Report(notification);


                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {

                IExceptionNotification notification =
                    ExceptionNotificationFactory.Instance.CreateNotification(
                        notSupportedException,
                        correlationIdentifier,
                        ServiceNotificationIdentifiers.ControllerTemplateGetNotSupportedException);
                _monitor.Report(notification);


                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (HttpResponseException responseException)
            {
                if (responseException.Response?.StatusCode != HttpStatusCode.NotFound)
                {

                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            responseException.InnerException ?? responseException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateGetException);
                    _monitor.Report(notification);

                }

                if (responseException.Response?.StatusCode == HttpStatusCode.NotFound)
                {
                    return new NotFoundResult();
                }

                throw;
            }
            catch (Exception exception)
            {
                IExceptionNotification notification =
                    ExceptionNotificationFactory.Instance.CreateNotification(
                        exception,
                        correlationIdentifier,
                        ServiceNotificationIdentifiers.ControllerTemplateGetException);
                _monitor.Report(notification);

                throw;
            }
        }
    }
}