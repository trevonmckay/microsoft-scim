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
    public class RootFunctions
    {
        private readonly ScimRootService _service;
        private readonly ILogger<RootFunctions> _logger;

        public RootFunctions(ScimRootService rootService, ILogger<RootFunctions> logger)
        {
            _service = rootService;
            _logger = logger;
        }

        // Get All
        [OpenApiOperation(operationId: "GetRootssRun", tags: new[] { "/scim" }, Summary = "Gets valid roots", Description = "This gets valid roots from AAD", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        [FunctionName(nameof(RootConstants.Roots))]
        public Task<HttpResponseMessage> GetUsersRun(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "scim")] HttpRequestMessage req,
            CancellationToken cancellationToken = default)
        {
            return _service.Get(req, cancellationToken);
        }


        // Get By Id
        //[FunctionName(nameof(RootConstants.GetRootsByAttributes))]
        //[OpenApiOperation(operationId: "GetRootsByIdRun", tags: new[] { "/scim" }, Summary = "Gets valid Roots", Description = "This gets valid Roots from AAD", Visibility = OpenApiVisibilityType.Important)]
        //[OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of Roots to return", Description = "Identifier of Roots to return", Visibility = OpenApiVisibilityType.Important)]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resource), Description = "The OK response with the Roots values")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Roots not found", Description = "Roots not found")]
        //public async Task<IActionResult> GetUserByIdRun(
        //  [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "scim/{identifier}")] HttpRequestMessage req,
        //  string identifier,
        //  CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"Get Resource by Identifier executed at: {DateTime.Now}");


        //        GetResourceCommand cmd = new GetResourceCommand(req, identifier);
        //        var userResult = await _mediator.Send(cmd, cancellationToken);


        //        return new OkObjectResult(userResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error creating a getting Roots: { ex.Message }");
        //        throw;
        //    }
        //}


        // Post
        //[FunctionName(nameof(RootConstants.PostRoots))]
        //[OpenApiOperation(operationId: "PostRootRun", tags: new[] { "/scim" }, Summary = "Posts valid Root Resource", Description = "This posts valid Root Resource from AAD", Visibility = OpenApiVisibilityType.Important)]
        //[OpenApiParameter("resourceData", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Root Resource to ADD", Description = "Root Resource to AAD", Visibility = OpenApiVisibilityType.Important)]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Core2Group), Description = "The OK response")]
        //public async Task<IActionResult> PostResourceRun(
        //  [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "scim/{resourceData}")] HttpRequestMessage req,
        //  string resourceData,
        //  CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"Create Resource executed at: {DateTime.Now}");
        //        Core2EnterpriseUser data = await req.Content.ReadAsAsync<Core2EnterpriseUser>();
        //        CreateResourceCommand cmd = new CreateResourceCommand(req, data);
        //        var userResult = await _mediator.Send(cmd, cancellationToken);


        //        return new OkObjectResult(userResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error creating a new Resource: { ex.Message }");
        //        throw;
        //    }
        //}


        // Put
        //[FunctionName(nameof(RootConstants.PutRoots))]
        //[OpenApiOperation(operationId: "PutRoots", tags: new[] { "/scim" }, Summary = "Gets valid Roots", Description = "This gets valid Roots from AAD", Visibility = OpenApiVisibilityType.Important)]
        //[OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of Roots to return", Description = "Identifier of Roots to return", Visibility = OpenApiVisibilityType.Important)]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resource), Description = "The OK response with the Roots values")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Roots not found", Description = "Roots not found")]
        //public static async Task<IActionResult> PutRootsRun(
        //  [HttpTrigger(AuthorizationLevel.Function, "put", Route = "scim")] HttpRequest req,
        //  ILogger log)
        //{
        //    log.LogInformation("C# HTTP trigger function processed a request.");

        //    string name = req.Query["name"];

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    name = name ?? data?.name;

        //    string responseMessage = string.IsNullOrEmpty(name)
        //        ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        //        : $"Hello, {name}. This HTTP triggered function executed successfully.";

        //    return new OkObjectResult(responseMessage);
        //}



        // Patch
        //[FunctionName(nameof(RootConstants.PatchRoots))]
        //[OpenApiOperation(operationId: "PatchRoots", tags: new[] { "/scim" }, Summary = "Gets valid Roots", Description = "This gets valid Roots from AAD", Visibility = OpenApiVisibilityType.Important)]
        //[OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of Roots to return", Description = "Identifier of Roots to return", Visibility = OpenApiVisibilityType.Important)]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resource), Description = "The OK response with the Roots values")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Roots not found", Description = "Roots not found")]
        //public static async Task<IActionResult> PatchRootsRun(
        //  [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "scim")] HttpRequest req,
        //  ILogger log)
        //{
        //    log.LogInformation("C# HTTP trigger function processed a request.");

        //    string name = req.Query["name"];

        //    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //    dynamic data = JsonConvert.DeserializeObject(requestBody);
        //    name = name ?? data?.name;

        //    string responseMessage = string.IsNullOrEmpty(name)
        //        ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        //        : $"Hello, {name}. This HTTP triggered function executed successfully.";

        //    return new OkObjectResult(responseMessage);
        //}



        // Delete
        //[FunctionName(nameof(RootConstants.DeleteRoots))]
        //[OpenApiOperation(operationId: "DeleteRoots", tags: new[] { "/scim" }, Summary = "Gets valid Roots", Description = "This gets valid Roots from AAD", Visibility = OpenApiVisibilityType.Important)]
        //[OpenApiParameter("identifier", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Identifier of Roots to return", Description = "Identifier of Roots to return", Visibility = OpenApiVisibilityType.Important)]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resource), Description = "The OK response with the Roots values")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid Identifier supplied", Description = "Invalid ID supplied")]
        //[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Roots not found", Description = "Roots not found")]
        //public async Task<IActionResult> DeleteRootsRun(
        //    [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "scim/{identifier}")] HttpRequestMessage req,
        //    string identifier,
        //    CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"Delete Resource by Identifier executed at: {DateTime.Now}");


        //        DeleteResourceCommand cmd = new DeleteResourceCommand(req, identifier);
        //        var userResult = await _mediator.Send(cmd, cancellationToken);


        //        return new OkObjectResult(userResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error deleting Roots: { ex.Message }");
        //        throw;
        //    }
        //}

    }
}
