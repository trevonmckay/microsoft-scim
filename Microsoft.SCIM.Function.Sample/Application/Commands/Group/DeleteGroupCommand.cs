using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Function.Application.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class DeleteGroupCommand : MediatR.IRequest<IActionResult>
    {
        public DeleteGroupCommand(HttpRequestMessage req, string identifier)
        {
            Identifier = identifier;
            this.Request = req;
        }

        public string Identifier { get; }

        public HttpRequestMessage Request { get; }
    }

    public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, IActionResult>
    {
        private readonly ILogger _logger;
        private readonly ISCIMService<Core2Group> _service;

        public DeleteGroupCommandHandler(IMonitor monitor,
                                         IProvider provider,
                                         ILogger<DeleteGroupCommandHandler> _logger)
        {
            this._logger = _logger;
            this._service = new GroupSCIMService(monitor, provider);
        }

        public Task<IActionResult> Handle(DeleteGroupCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(command.Identifier))
            {
                this._logger.LogInformation("Error: Deleting Group with empty/null identifier");
                throw new HttpRequestException();
            }


            return this._service.Delete(command.Request, command.Identifier, correlationIdentifier: null);
        }
    }
}