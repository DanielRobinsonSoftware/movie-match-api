using System.Net.Http;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using MovieMatch.Identity;

namespace MovieMatch
{
    public class EndpointBase
    {   
        protected readonly HttpClient HttpClient;
        protected readonly string MovieDbAccessToken;
        protected const string MovieDbBaseUri = "https://api.themoviedb.org";
        protected readonly AzureADJwtBearerValidation AzureADJwtBearerValidation;

        protected EndpointBase(IHttpClientFactory httpClientFactory, AzureADJwtBearerValidation azureADJwtBearerValidation)
        {
            HttpClient = httpClientFactory.CreateClient();
            MovieDbAccessToken = GetMovieDbAccessToken();
            AzureADJwtBearerValidation = azureADJwtBearerValidation;
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
