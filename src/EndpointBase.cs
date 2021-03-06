using System.Net.Http;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using MovieMatch.Identity;

namespace MovieMatch
{
    public class EndpointBase
    {   
        protected IHttpClientFactory HttpClientFactory { get; private set; }
        protected IAzureADJwtBearerValidation AzureADJwtBearerValidation { get; private set; }
        protected const string MovieDbBaseUri = "https://api.themoviedb.org";

        protected EndpointBase(IHttpClientFactory httpClientFactory, IAzureADJwtBearerValidation azureADJwtBearerValidation)
        {
            HttpClientFactory = httpClientFactory;
            AzureADJwtBearerValidation = azureADJwtBearerValidation;
        }
        
        protected string GetMovieDbAccessToken()
        {
            var keyVaultUri = Guard.NotNull("KeyVaultUri", Environment.GetEnvironmentVariable("KeyVaultUri"));

            var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
            var secretResponse = secretClient.GetSecret("MovieDBAccessToken");
            return secretResponse.Value.Value;
        }
    }
}
