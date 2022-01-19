using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace MovieMatch.Identity
{
    public class AzureADJwtBearerValidation
    {
        private IConfiguration _configuration;
        private ILogger _log;
        private ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
 
        private string _wellKnownEndpoint;
        private string _audience;
 
        public AzureADJwtBearerValidation(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _log = loggerFactory.CreateLogger<AzureADJwtBearerValidation>();
 
            var instance = _configuration["AzureADInstance"];
            var tenantId = _configuration["AzureADTenantId"];
            _wellKnownEndpoint = $"{instance}{tenantId}/v2.0/.well-known/openid-configuration";
            _audience = _configuration["ApiApplicationId"];
        }
 
        public async Task<bool> ValidateTokenAsync(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                _log.LogDebug("Token validation failed due to missing authorization header");
                return false;
            }
 
            if (!authorizationHeader.Contains("Bearer"))
            {
                _log.LogDebug("Token validation failed due to missing Bearer token");
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
                _log.LogDebug($"Token validation failed due to {ex.ToString()}");
            }
            return false;
        }
 
        private async Task<OpenIdConnectConfiguration> GetOIDCWellknownConfiguration()
        {
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                 _wellKnownEndpoint, new OpenIdConnectConfigurationRetriever());
 
            return await _configurationManager.GetConfigurationAsync();
        }
    }
}