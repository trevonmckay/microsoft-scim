using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Function.Application.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class CreateGroupCommand : MediatR.IRequest<IActionResult>
    {
        public CreateGroupCommand(HttpRequestMessage request, Core2Group resource)
        {
            Request = request;
            Resource = resource;
        }

        public HttpRequestMessage Request { get; }

        public Core2Group Resource { get; }
    }

    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, IActionResult>
    {
        private readonly ILogger<CreateGroupCommandHandler> _logger;
        private readonly ISCIMService<Core2Group> _service;


        public CreateGroupCommandHandler(IMonitor monitor,
                                         IProvider provider,
                                         ILogger<CreateGroupCommandHandler> _logger)
        {
            this._logger = _logger;
            this._service = new GroupSCIMService(monitor, provider);
        }


        public Task<IActionResult> Handle(CreateGroupCommand command, CancellationToken cancellationToken)
        {
            return this._service.PostAsync(command.Request, command.Resource);
        }
    }
}
