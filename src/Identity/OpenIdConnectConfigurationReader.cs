using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace MovieMatch.Identity
{
    public class OpenIdConnectConfigurationReader : IOpenIdConnectConfigurationReader
    {
        public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(string wellKnownEndpoint)
        {
            ConfigurationManager<OpenIdConnectConfiguration> configurationManager = 
                new ConfigurationManager<OpenIdConnectConfiguration>(wellKnownEndpoint, new OpenIdConnectConfigurationRetriever());
 
            return await configurationManager.GetConfigurationAsync();
        }
    }
}