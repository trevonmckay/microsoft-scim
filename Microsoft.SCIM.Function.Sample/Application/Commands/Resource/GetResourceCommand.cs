using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Function.Application.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class GetResourceCommand : MediatR.IRequest<ActionResult<QueryResponseBase>>
    {
        public GetResourceCommand(HttpRequestMessage req, string identifier)
        {
            Identifier = identifier;
            this.Request = req;
        }

        public string Identifier { get; }

        public HttpRequestMessage Request { get; }
    }

    public class GetResourceCommandHandler : IRequestHandler<GetResourceCommand, ActionResult<QueryResponseBase>>
    {
        private readonly ILogger _logger;
        private readonly ISCIMService<Resource> _service;

        public GetResourceCommandHandler(IMonitor monitor,
                                         IProvider provider,
                                         ILogger<GetGroupCommandHandler> _logger)
        {
            this._logger = _logger;
            this._service = new ResourceSCIMService(monitor, provider);
        }

        public Task<ActionResult<QueryResponseBase>> Handle(GetResourceCommand command, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(command.Identifier))
            {
                return this._service.GetAsync(command.Request, command.Identifier);
            }

            return this._service.GetAsync(command.Request);
        }
    }
}
