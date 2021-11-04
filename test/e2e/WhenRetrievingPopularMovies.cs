using System;
using Xunit;
using RestSharp;
using FluentAssertions;

namespace e2e
{
    public class WhenRetrievingPopularMovies
    {
        // TODO: Given I have authenticated

        [Fact]
        public void ThenResponseIsSuccessful()
        {
            // TODO: read app name from env variable
            var client = new RestClient("https://moviematch211027-staging.azurewebsites.net/api/v1/");

            var request = new RestRequest("movies/popular");

            var response = client.Get(request);

            response.IsSuccessful.Should().BeTrue();
        }

        // TODO: Tests to validate the expected structure of the response content
    }
}
