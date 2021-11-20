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
    public class UserFunctions
    {
        private readonly ScimUsersService _service;
        private readonly ILogger<UserFunctions> _logger;

        public UserFunctions(ScimUsersService usersService, ILogger<UserFunctions> logger)
        {
            _service = usersService;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: UserConstants.QueryUsersFunctionName, tags: new[] { "/scim/users" }, Summary = "Get users", Description = "Returns list of users from the service provider", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        [FunctionName(UserConstants.QueryUsersFunctionName)]
        public Task<HttpResponseMessage> GetUsersRun(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "scim/users")] HttpRequestMessage req,
            CancellationToken cancellationToken = default)
        {
            return _service.Get(req, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="identifier"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: UserConstants.GetUserFunctionName, tags: new[] { "/scim/users" }, Summary = "Get user for identifier", Description = "Returns the user for the given identifier", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of user to return", Description = "Identifier of user to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resource), Description = "The OK response with the user values")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "User not found", Description = "User not found")]
        [FunctionName(UserConstants.GetUserFunctionName)]
        public Task<HttpResponseMessage> GetUserByIdRun(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "scim/users/{identifier}")] HttpRequestMessage req,
          string identifier,
          CancellationToken cancellationToken = default)
        {
            return _service.Get(req, identifier, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resource"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: UserConstants.CreateUserFunctionName, tags: new[] { "/scim/users" }, Summary = "Posts valid User", Description = "This posts valid User from AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("resourceData", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "User to ADD", Description = "User to AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Core2EnterpriseUser), Description = "The OK response")]
        [FunctionName(UserConstants.CreateUserFunctionName)]
        public Task<HttpResponseMessage> PostUserRun(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "scim/users")] HttpRequestMessage req,
          CancellationToken cancellationToken = default)
        {
            return _service.Post(req, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="identifier"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: UserConstants.UpdateUserFunctionName, tags: new[] { "/scim/users" }, Summary = "Gets valid User", Description = "This gets valid User from AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of User to return", Description = "Identifier of User to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Core2EnterpriseUser), Description = "The OK response with the User values")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "User not found", Description = "User not found")]
        [FunctionName(UserConstants.UpdateUserFunctionName)]
        public Task<HttpResponseMessage> PatchUserRun(
          [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "scim/users/{identifier}")] HttpRequestMessage req,
          string identifier,
          CancellationToken cancellationToken = default)
        {
            return _service.Patch(req, identifier, cancellationToken);
        }

        /// <summary>
        /// Replaces a user
        /// </summary>
        /// <param name="req"></param>
        /// <param name="identifier"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: UserConstants.ReplaceUserFunctionName, tags: new[] { "/scim/users" }, Summary = "Gets valid Group", Description = "This gets valid Group from AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of Group to return", Description = "Identifier of Group to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resource), Description = "The OK response with the Group values")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Group not found", Description = "Group not found")]
        [FunctionName(UserConstants.ReplaceUserFunctionName)]
        public Task<HttpResponseMessage> PutGroupRun(
          [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "scim/users/{identifier}")] HttpRequestMessage req,
          string identifier,
          CancellationToken cancellationToken = default)
        {
            return _service.Put(req, identifier, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="identifier"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [OpenApiOperation(operationId: UserConstants.DeleteUserFunctionName, tags: new[] { "/scim/users" }, Summary = "Gets valid User", Description = "This gets valid User from AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of User to return", Description = "Identifier of Roots to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resource), Description = "The OK response with the Roots values")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "User not found", Description = "User not found")]
        [FunctionName(UserConstants.DeleteUserFunctionName)]
        public Task<HttpResponseMessage> DeleteUsersRun(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "scim/users/{identifier}")] HttpRequestMessage req,
            string identifier,
            CancellationToken cancellationToken = default)
        {
            return _service.Delete(req, identifier, cancellationToken);
        }
    }
}

