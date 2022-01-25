using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace MovieMatch.Identity
{
    public interface IOpenIdConnectConfigurationReader
    {
        Task<OpenIdConnectConfiguration> GetConfigurationAsync(string wellKnownEndpoint);
    }
}