using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Microsoft.SCIM
{
    public abstract class BaseScimService : IScimService
    {
        internal const string AttributeValueIdentifier = "{identifier}";

        internal readonly IMonitor monitor;
        internal readonly IProvider provider;

        internal BaseScimService(IProvider provider, IMonitor monitor)
        {
            this.monitor = monitor;
            this.provider = provider;
        }

        protected virtual Uri GetResourceLocationUri(HttpRequestMessage request, Resource resource)
        {
            Uri baseResourceIdentifier = request.GetBaseResourceIdentifier();
            return resource.GetResourceIdentifier(baseResourceIdentifier);
        }

        protected virtual string GetResourceLocation(HttpRequestMessage request, Resource resource)
        {
            return GetResourceLocationUri(request, resource).AbsoluteUri;
        }

        protected virtual bool TryGetMonitor(out IMonitor monitor)
        {
            monitor = this.monitor;
            if (null == monitor)
            {
                return false;
            }

            return true;
        }

        protected virtual HttpResponseMessage BadRequest(object result = null)
        {
            return CreateResponse(HttpStatusCode.BadRequest, result);
        }

        protected virtual HttpResponseMessage Created(Uri uri, Resource resource = null)
        {
            HttpResponseMessage response = CreateResponse(HttpStatusCode.Created, resource, ProtocolConstants.ContentType);
            return response;
        }

        protected virtual HttpResponseMessage Created(string uri, Resource resource = null)
        {
            return Created(new Uri(uri), resource);
        }

        protected virtual HttpResponseMessage Conflict(object result = null)
        {
            return CreateResponse(HttpStatusCode.Conflict, result);
        }

        protected virtual HttpResponseMessage InternalServerError()
        {
            return CreateResponse(HttpStatusCode.InternalServerError);
        }

        protected virtual HttpResponseMessage NotFound(object result = null)
        {
            return CreateResponse(HttpStatusCode.NotFound, result);
        }

        protected virtual HttpResponseMessage NoContent()
        {
            return CreateResponse(HttpStatusCode.NoContent);
        }

        protected virtual HttpResponseMessage NotImplemented()
        {
            return CreateResponse(HttpStatusCode.NotImplemented);
        }

        protected virtual HttpResponseMessage Ok(object result = null)
        {
            return CreateResponse(HttpStatusCode.OK, result);
        }

        protected virtual HttpResponseMessage TooManyRequests(object result = null)
        {
            return CreateResponse(HttpStatusCode.TooManyRequests, result);
        }

        protected HttpResponseMessage CreateResponse(HttpStatusCode statusCode, object result = null, string contentType = "application/json")
        {
            HttpResponseMessage response = new(statusCode);
            if (result is not null)
            {
                string responseBody = JsonConvert.SerializeObject(result);
                response.Content = new StringContent(responseBody, Encoding.UTF8, contentType);
            }

            return response;
        }
    }
}
