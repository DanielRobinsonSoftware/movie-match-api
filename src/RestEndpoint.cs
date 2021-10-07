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
    public class RestEndpoint : EndpointBase
    {
        public RestEndpoint(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        [FunctionName("RestEndpoint")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
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
