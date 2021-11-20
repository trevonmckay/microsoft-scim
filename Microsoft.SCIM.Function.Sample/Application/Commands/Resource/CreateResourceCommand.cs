using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SCIM.Function.Application.Services;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class CreateResourceCommand : MediatR.IRequest<IActionResult>
    {
        public CreateResourceCommand(HttpRequestMessage request, Resource resource)
        {
            Request = request;
            Resource = resource;
        }

        public HttpRequestMessage Request { get; }

        public Resource Resource { get; }
    }

    public class CreateResourceCommandHandler : IRequestHandler<CreateResourceCommand, IActionResult>
    {
        private readonly ILogger _logger;
        private readonly ISCIMService<Resource> _service;

        public CreateResourceCommandHandler(IMonitor monitor,
                                         IProvider provider,
                                         ILogger<CreateResourceCommandHandler> _logger)
        {
            this._logger = _logger;
            this._service = new ResourceSCIMService(monitor, provider);
        }

        public Task<IActionResult> Handle(CreateResourceCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Posting resource with id: {command.Resource.ExternalIdentifier}");
            return this._service.PostAsync(command.Request, command.Resource);
        }
    }
}