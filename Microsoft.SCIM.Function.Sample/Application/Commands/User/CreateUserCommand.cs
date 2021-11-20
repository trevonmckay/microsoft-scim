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
    public class CreateUserCommand : MediatR.IRequest<IActionResult>
    {
        public CreateUserCommand(HttpRequestMessage req, Resource resource, string identifier = null)
        {
            this.Request = req;
            this.Resource = resource;
            this.Identifier = identifier;
        }

        public HttpRequestMessage Request { get; }

        public Resource Resource { get; }

        public string Identifier { get; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IActionResult>
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

        public CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger,
                                        IMonitor monitor,
                                        IProvider provider)
        {
            this._logger = logger;
            this._service = new UserSCIMService(monitor, provider);
        }

        public Task<IActionResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Posting resource with id: {command.Resource.ExternalIdentifier}");
            Core2EnterpriseUser user = command.Resource as Core2EnterpriseUser;

            return this._service.PostAsync(command.Request, user);
        }
    }
}