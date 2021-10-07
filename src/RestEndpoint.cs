using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MovieMatch
{
    public class MovieEndpoints : EndpointBase
    {
        public MovieEndpoints(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        [FunctionName("v1/movie/popular")]
        public async Task<IActionResult> Popular(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var response = await HttpClient.GetWithHeadersAsync("https://api.themoviedb.org/3/movie/popular", new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {MovieDbAccessToken}"
            });

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            return new OkObjectResult(responseContent);
        }
    }
}
