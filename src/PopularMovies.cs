using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MovieMatch.Identity;
using System;

namespace MovieMatch
{
    public class PopularMovies : EndpointBase
    {
        public PopularMovies(IHttpClientFactory httpClientFactory, IAzureADJwtBearerValidation azureADJwtBearerValidation) 
            : base(httpClientFactory, azureADJwtBearerValidation)
        {
        }

        [FunctionName("PopularMovies")]
        public async Task<IActionResult> GetPopularMovies(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/movies/popular")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HttpTrigger GetPopularMovies activated");

            try
            {
                // TODO: Refactor for reuse
                var authorizationHeader = req.Headers["Authorization"];
                var validToken = await AzureADJwtBearerValidation.ValidateTokenAsync(authorizationHeader);
                if (!validToken)
                {
                    return new UnauthorizedResult();
                }

                var movieDbAccessToken = GetMovieDbAccessToken();
                log.LogTrace($"Retrieved {nameof(movieDbAccessToken)}, with length of {movieDbAccessToken?.Length ?? 0}");

                var httpClient = HttpClientFactory.CreateClient();
                var response = await httpClient.GetWithAuthHeaderAsync($"{MovieDbBaseUri}/3/movie/popular", movieDbAccessToken);
                log.LogTrace($"Received {response?.StatusCode.ToString() ?? "null"} response from MovieDB");
                
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

                return new OkObjectResult(responseContent);
            }
            catch(Exception ex)
            {
                log.LogError($"Exception thrown during HttpTrigger GetPopularMovies {ex.Message} {ex.StackTrace}");
                throw ex;
            }
        }
    }
}
