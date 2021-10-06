using System.Net.Http;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;

namespace MovieMatch
{
    public class EndpointBase
    {   
        protected readonly HttpClient HttpClient;
        protected readonly string MovieDbAccessToken;

        protected EndpointBase(IHttpClientFactory httpClientFactory)
        {
            HttpClient = httpClientFactory.CreateClient();
            MovieDbAccessToken = GetMovieDbAccessToken();
        }
        
        private string GetMovieDbAccessToken()
        {
            var keyVaultUri = Guard.NotNull("KeyVaultUri", Environment.GetEnvironmentVariable("KeyVaultUri"));

            var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
            var secretResponse = secretClient.GetSecret("MovieDBAccessToken");
            return secretResponse.Value.Value;
        }
    }
}
