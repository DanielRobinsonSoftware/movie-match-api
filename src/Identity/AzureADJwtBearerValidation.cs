using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace MovieMatch.Identity
{
    // Based on Damien Bod's example
    // https://damienbod.com/2020/09/24/securing-azure-functions-using-azure-ad-jwt-bearer-token-authentication-for-user-access-tokens/
    public class AzureADJwtBearerValidation
    {
        private IConfiguration _configuration;
        private ILogger _log;
        private const string scopeType = @"http://schemas.microsoft.com/identity/claims/scope";
        private ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private ClaimsPrincipal _claimsPrincipal;
 
        private string _wellKnownEndpoint = string.Empty;
        private string _tenantId = string.Empty;
        private string _audience = string.Empty;
        private string _instance = string.Empty;
 
        public AzureADJwtBearerValidation(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _log = loggerFactory.CreateLogger<AzureADJwtBearerValidation>();
 
            _tenantId = _configuration["AzureADTenantId"];
            _audience = _configuration["ApiApplicationId"];
            _instance = _configuration["AzureADInstance"];
            _wellKnownEndpoint = $"{_instance}{_tenantId}/v2.0/.well-known/openid-configuration";
        }
 
        public async Task<bool> ValidateTokenAsync(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return false;
            }
 
            if (!authorizationHeader.Contains("Bearer"))
            {
                return false;
            }
 
            var accessToken = authorizationHeader.Substring("Bearer ".Length);
 
            var oidcWellknownEndpoints = await GetOIDCWellknownConfiguration();
  
            var tokenValidator = new JwtSecurityTokenHandler();
 
            var validationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidAudience = _audience,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKeys = oidcWellknownEndpoints.SigningKeys,
                ValidIssuer = oidcWellknownEndpoints.Issuer
            };
 
            try
            {
                SecurityToken securityToken;                
                tokenValidator.ValidateToken(accessToken, validationParameters, out securityToken);

                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToString());
            }
            return false;
        }
 
        public string GetPreferredUserName()
        {
            string preferredUsername = string.Empty;
            var preferred_username = _claimsPrincipal.Claims.FirstOrDefault(t => t.Type == "preferred_username");
            if (preferred_username != null)
            {
                preferredUsername = preferred_username.Value;
            }
 
            return preferredUsername;
        }
 
        private async Task<OpenIdConnectConfiguration> GetOIDCWellknownConfiguration()
        {
            _log.LogDebug($"Get OIDC well known endpoints {_wellKnownEndpoint}");
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                 _wellKnownEndpoint, new OpenIdConnectConfigurationRetriever());
 
            return await _configurationManager.GetConfigurationAsync();
        }
    }
}