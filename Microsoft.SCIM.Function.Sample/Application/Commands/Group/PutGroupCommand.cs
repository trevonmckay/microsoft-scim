using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class PutGroupCommand : MediatR.IRequest<ActionResult<QueryResponseBase>>
    {
        public PutGroupCommand(HttpRequestMessage request, Resource resource)
        {
            Request = request;
            Resource = resource;
        }

        public HttpRequestMessage Request { get; }

        public Resource Resource { get; }
    }
}