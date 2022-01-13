using System;
using Xunit;
using Xunit.Abstractions;
using RestSharp;
using FluentAssertions;
using Newtonsoft.Json;

namespace e2e
{
    public class WhenRetrievingPopularMovies
    {
        private RestClient _apiClient;
        private string _bearerToken;
        private readonly ITestOutputHelper _output;

        public WhenRetrievingPopularMovies(ITestOutputHelper output)
        {
            _output = output;

            var apiAppName = Environment.GetEnvironmentVariable("API_APP_NAME");
            _apiClient = new RestClient($"https://{apiAppName}-staging.azurewebsites.net/api/v1/");

            GivenIHaveAuthenticated();
        }

        private void GivenIHaveAuthenticated()
        {
            // https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-client-creds-grant-flow
            var instance = Environment.GetEnvironmentVariable("AZURE_AD_INSTANCE");
            var tenantId = Environment.GetEnvironmentVariable("AZURE_AD_TENANT_ID");
            var tokenClient = new RestClient($"{instance}{tenantId}/oauth2/v2.0/token");
            var request = new RestRequest();
            
            request.AddParameter("client_id", Environment.GetEnvironmentVariable("WEB_APP_CLIENT_ID")); // The web app's application ID.
            request.AddParameter("scope", Environment.GetEnvironmentVariable("API_SCOPE")); // The resource identifier (application ID URI) of the resource you want, affixed with the .default suffix. 
            request.AddParameter("client_secret", Environment.GetEnvironmentVariable("WEB_APP_CLIENT_SECRET")); // The client secret generated for the app in the app registration portal.
            request.AddParameter("grant_type", "client_credentials"); // Must be set to client_credentials.

            var response = tokenClient.Post(request);

            if (!response.IsSuccessful)
            {
                throw new Exception("Could not obtain authentication token");
            }

            _bearerToken = JsonConvert.DeserializeObject<TokenResponse>(response.Content).AccessToken;
        }

        [Fact]
        public void ThenResponseIsSuccessful()
        {
            var request = new RestRequest("movies/popular");
            request.AddHeader("Authorization", $"Bearer {_bearerToken}");

            // Act
            var response = _apiClient.Get(request);

            // Assert
            _output.WriteLine($"Reponse status is: {response.StatusCode.ToString()} {response.StatusDescription}");
            response.IsSuccessful.Should().BeTrue();            
        }

        // TODO: Tests to validate the expected structure of the response content
    }
}
