using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MovieMatch
{
    public class PopularMovies : EndpointBase
    {
        public PopularMovies(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        [FunctionName("PopularMovies")]
        public async Task<IActionResult> GetPopularMovies(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/movies/popular")] HttpRequest req,
            ILogger log)
        {
            var response = await HttpClient.GetWithAuthHeaderAsync($"{MovieDbBaseUri}/3/movie/popular", MovieDbAccessToken);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            return new OkObjectResult(responseContent);
        }
    }
}
