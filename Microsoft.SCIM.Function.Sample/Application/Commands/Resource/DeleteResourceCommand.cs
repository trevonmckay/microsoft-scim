using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Function.Application.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class DeleteResourceCommand : MediatR.IRequest<IActionResult>
    {
        public DeleteResourceCommand(HttpRequestMessage req, string identifier)
        {
            Identifier = identifier;
            this.Request = req;
        }

        public string Identifier { get; }

        public HttpRequestMessage Request { get; }
    }

    public class DeleteResourceCommandHandler : IRequestHandler<DeleteResourceCommand, IActionResult>
    {
        private readonly ILogger _logger;
        private readonly ISCIMService<Resource> _service;

        public DeleteResourceCommandHandler(IMonitor monitor,
                                         IProvider provider,
                                         ILogger<DeleteResourceCommandHandler> _logger)
        {
            this._logger = _logger;
            this._service = new ResourceSCIMService(monitor, provider);
        }

        public Task<IActionResult> Handle(DeleteResourceCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(command.Identifier))
            {
                this._logger.LogInformation("Error: Deleting Resource with empty/null identifier");
                throw new HttpRequestException();
            }


            return this._service.Delete(command.Request, command.Identifier, correlationIdentifier: null);
        }
    }
}