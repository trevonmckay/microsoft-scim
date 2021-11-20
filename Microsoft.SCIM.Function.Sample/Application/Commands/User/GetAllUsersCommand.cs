using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Function.Application.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class GetAllUsersCommand : MediatR.IRequest<ActionResult<QueryResponseBase>>
    {
        public GetAllUsersCommand(HttpRequestMessage req, string correlationIdentifier)
        {
            CorrelationIdentifier = correlationIdentifier;
            this.Request = req;
        }

        public string CorrelationIdentifier { get; }

        public HttpRequestMessage Request { get; }
    }

    public class GetAllUsersCommandHandler : IRequestHandler<GetUserByIdentifierCommand, ActionResult<QueryResponseBase>>
    {
        private readonly ILogger _logger;
        private readonly ISCIMService<Core2EnterpriseUser> _service;

        public GetAllUsersCommandHandler(IMonitor monitor,
                                         IProvider provider,
                                         ILogger<GetAllUsersCommandHandler> _logger)
        {
            this._logger = _logger;
            this._service = new UserSCIMService(monitor, provider);
        }

        public Task<ActionResult<QueryResponseBase>> Handle(GetUserByIdentifierCommand command, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(command.Identifier))
            {
                return this._service.GetAsync(command.Request, command.Identifier);
            }

            return this._service.GetAsync(command.Request);
        }
    }
}
