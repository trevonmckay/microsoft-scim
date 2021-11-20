using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Function.Application.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class DeleteUserCommand : MediatR.IRequest<IActionResult>
    {
        public DeleteUserCommand(HttpRequestMessage req, string identifier)
        {
            Identifier = identifier;
            this.Request = req;
        }

        public string Identifier { get; }

        public HttpRequestMessage Request { get; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IActionResult>
    {
        private readonly ILogger _logger;
        private readonly ISCIMService<Core2EnterpriseUser> _service;

        public DeleteUserCommandHandler(IMonitor monitor,
                                         IProvider provider,
                                         ILogger<DeleteUserCommandHandler> _logger)
        {
            this._logger = _logger;
            this._service = new UserSCIMService(monitor, provider);
        }

        public Task<IActionResult> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(command.Identifier))
            {
                this._logger.LogInformation("Error: Deleting User with empty/null identifier");
                throw new HttpRequestException();
            }

            return this._service.Delete(command.Request, command.Identifier, correlationIdentifier: null);
        }
    }
}
