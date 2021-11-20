using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Function.Application.Services
{
    public interface ISCIMService<T> where T : Resource
    {
        Task<ActionResult<QueryResponseBase>> GetAsync(HttpRequestMessage Request);

        Task<ActionResult<QueryResponseBase>> GetAsync(HttpRequestMessage request, string identifier);

        Task<IActionResult> PostAsync(HttpRequestMessage request, T resource);

        Task<IActionResult> PutAsync(HttpRequestMessage request, T patchRequest, string identifier);

        Task<ActionResult<QueryResponseBase>> PatchAsync(HttpRequestMessage request, PatchRequest2 patchRequest, string identifier);

        Task<IActionResult> Delete(HttpRequestMessage request, string identifier, string correlationIdentifier);
    }
}
