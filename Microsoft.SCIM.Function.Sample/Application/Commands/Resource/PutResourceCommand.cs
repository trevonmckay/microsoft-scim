using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Microsoft.SCIM.Sample.Application.Commands
{
    public class PutResourceCommand : MediatR.IRequest<ActionResult<QueryResponseBase>>
    {
        public PutResourceCommand(HttpRequestMessage request, Resource resource)
        {
            Request = request;
            Resource = resource;
        }

        public HttpRequestMessage Request { get; }

        public Resource Resource { get; }
    }
}