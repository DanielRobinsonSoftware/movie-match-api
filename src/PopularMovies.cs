using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using MovieMatch.Identity;

namespace MovieMatch
{
    public class PopularMovies : EndpointBase
    {
        public PopularMovies(IHttpClientFactory httpClientFactory, AzureADJwtBearerValidation azureADJwtBearerValidation) 
            : base(httpClientFactory, azureADJwtBearerValidation)
        {
        }

        [FunctionName("PopularMovies")]
        public async Task<IActionResult> GetPopularMovies(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/movies/popular")] HttpRequest req,
            ILogger log)
        {
            // TODO: Refactor for reuse
            ClaimsPrincipal principal = await AzureADJwtBearerValidation.ValidateTokenAsync(req.Headers["Authorization"]);
            if (principal == null)
            {
                return new UnauthorizedResult();
            }

            var response = await HttpClient.GetWithAuthHeaderAsync($"{MovieDbBaseUri}/3/movie/popular", MovieDbAccessToken);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            return new OkObjectResult(responseContent);
        }
    }
}
