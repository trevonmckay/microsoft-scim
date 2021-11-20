using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Function.Functions
{
    public class TokenFunctions
    {
        public const string GetToken = "GetToken";

        private readonly ILogger<TokenFunctions> _logger;

        public TokenFunctions(ILogger<TokenFunctions> logger)
        {
            _logger = logger;
        }

        [OpenApiOperation(operationId: "GetTokenRun", tags: new[] { "token" }, Summary = "Gets valid Token", Description = "This gets valid token for authorization.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "It returns token: valid authorization token")]
        [FunctionName(nameof(GetToken))]
        public async Task<IActionResult> GetTokenRun(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "token")] HttpRequest req,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Get Token executed at: {DateTime.Now}");

            return new OkResult();
        }
    }
}
