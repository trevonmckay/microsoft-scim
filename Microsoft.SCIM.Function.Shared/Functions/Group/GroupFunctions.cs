using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Function.Functions
{
    public class GroupFunctions
    {
        private readonly ScimGroupsService _service;
        private readonly ILogger<GroupFunctions> _logger;

        public GroupFunctions(ScimGroupsService groupsService, ILogger<GroupFunctions> logger)
        {
            _service = groupsService;
            _logger = logger;
        }

        /// <summary>
        /// Returns all groups
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: GroupConstants.QueryGroupsFunctionName, tags: new[] { "/scim/group" }, Summary = "Gets valid group", Description = "This gets valid group from AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        [FunctionName(GroupConstants.QueryGroupsFunctionName)]
        public Task<HttpResponseMessage> GetGroupsRun(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "scim/group")] HttpRequestMessage req,
            CancellationToken cancellationToken = default)
        {
            return _service.Get(req, cancellationToken);
        }

        /// <summary>
        /// Get resource by identifier
        /// </summary>
        /// <param name="req"></param>
        /// <param name="identifier"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: GroupConstants.GetGroupFunctionName, tags: new[] { "/scim/group" }, Summary = "Gets valid Group", Description = "This gets valid Group from AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of Group to return", Description = "Identifier of Group to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resource), Description = "The OK response with the Group values")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Group not found", Description = "Group not found")]
        [FunctionName(GroupConstants.GetGroupFunctionName)]
        public Task<HttpResponseMessage> GetGroupByIdRun(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "scim/group/{identifier}")] HttpRequestMessage req,
          string identifier,
          CancellationToken cancellationToken = default)
        {
            return _service.Get(req, identifier, cancellationToken);
        }

        /// <summary>
        /// Create group
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: GroupConstants.CreateGroupFunctionName, tags: new[] { "/scim/group" }, Summary = "Posts valid Group", Description = "This posts valid Group from AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("resourceData", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Group to ADD", Description = "Group to AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Core2Group), Description = "The OK response")]
        [FunctionName(GroupConstants.CreateGroupFunctionName)]
        public Task<HttpResponseMessage> PostGroupRun(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "scim/group/{resourceData}")] HttpRequestMessage req,
          CancellationToken cancellationToken = default)
        {
            return _service.Post(req, cancellationToken);
        }

        /// <summary>
        /// Updates a group
        /// </summary>
        /// <param name="req"></param>
        /// <param name="identifier"></param>
        /// <param name="patchRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: GroupConstants.UpdateGroupFunctionName, tags: new[] { "/scim/group" }, Summary = "Gets valid Group", Description = "This gets valid Group from AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of Group to return", Description = "Identifier of Group to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resource), Description = "The OK response with the Group values")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Group not found", Description = "Group not found")]
        [FunctionName(GroupConstants.UpdateGroupFunctionName)]
        public Task<HttpResponseMessage> PatchGroupRun(
          [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "scim/group/{identifier}")] HttpRequestMessage req,
          string identifier,
          CancellationToken cancellationToken = default)
        {
            return _service.Patch(req, identifier, cancellationToken);
        }

        /// <summary>
        /// Replaces a group
        /// </summary>
        /// <param name="req"></param>
        /// <param name="identifier"></param>
        /// <param name="resourceData"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: GroupConstants.ReplaceGroupFunctionName, tags: new[] { "/scim/group" }, Summary = "Gets valid Group", Description = "This gets valid Group from AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of Group to return", Description = "Identifier of Group to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resource), Description = "The OK response with the Group values")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Group not found", Description = "Group not found")]
        [FunctionName(GroupConstants.ReplaceGroupFunctionName)]
        public Task<HttpResponseMessage> PutGroupRun(
          [HttpTrigger(AuthorizationLevel.Function, "put", Route = "scim/group/{identifier}")] HttpRequestMessage req,
          string identifier,
          CancellationToken cancellationToken = default)
        {
            return _service.Put(req, identifier, cancellationToken);
        }

        /// <summary>
        /// Deletes a group
        /// </summary>
        /// <param name="req"></param>
        /// <param name="identifier"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: GroupConstants.DeleteGroupFunctionName, tags: new[] { "/scim/group" }, Summary = "Gets valid Group", Description = "This gets valid Group from AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of Group to return", Description = "Identifier of Group to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resource), Description = "The OK response with the Group values")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Group not found", Description = "Group not found")]
        [FunctionName(GroupConstants.DeleteGroupFunctionName)]
        public Task<HttpResponseMessage> DeleteGroupRun(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "scim/group/{identifier}")] HttpRequestMessage req,
            string identifier,
            CancellationToken cancellationToken = default)
        {
            return _service.Delete(req, identifier, cancellationToken);
        }
    }
}

