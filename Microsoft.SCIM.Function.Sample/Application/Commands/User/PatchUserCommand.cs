using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Function.Application.Services;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class PatchUserCommand : MediatR.IRequest<ActionResult<QueryResponseBase>>
    {
        public PatchUserCommand(HttpRequestMessage req, PatchRequest2 resource, string identifier = null)
        {
            this.Request = req;
            this.Resource = resource;
            this.Identifier = identifier;
        }

        public HttpRequestMessage Request { get; }

        public PatchRequest2 Resource { get; }

        public string Identifier { get; }
    }

    public class PatchUserCommandHandler : IRequestHandler<PatchUserCommand, ActionResult<QueryResponseBase>>
    {
        private readonly ILogger _logger;
        private readonly ISCIMService<Core2EnterpriseUser> _service;

        protected IProviderAdapter<Core2EnterpriseUser> AdaptProvider(IProvider provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Core2EnterpriseUser> result = new Core2EnterpriseUserProviderAdapter(provider);
            return result;
        }

        public PatchUserCommandHandler(ILogger<PatchUserCommandHandler> logger,
                                        IMonitor monitor,
                                        IProvider provider)
        {
            this._logger = logger;
            this._service = new UserSCIMService(monitor, provider);
        }

        public Task<ActionResult<QueryResponseBase>> Handle(PatchUserCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Patching resource with id: {command.Identifier}");
            return this._service.PatchAsync(command.Request, command.Resource, command.Identifier);
        }
    }
}