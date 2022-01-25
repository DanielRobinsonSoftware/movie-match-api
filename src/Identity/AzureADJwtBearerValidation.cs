using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MovieMatch.Identity
{
    public class AzureADJwtBearerValidation : IAzureADJwtBearerValidation
    {
        private ISecurityTokenValidator _tokenValidator;
        private IConfiguration _configuration;
        private ILogger _log;
        private IOpenIdConnectConfigurationReader _openIdConnectConfigurationReader;

        private string _wellKnownEndpoint;
        private string _audience;
 
        public AzureADJwtBearerValidation(
            ISecurityTokenValidator tokenValidator,
            IConfiguration configuration,
            IOpenIdConnectConfigurationReader openIdConnectConfigurationReader,
            ILoggerFactory loggerFactory)
        {
            _tokenValidator = Guard.NotNull(nameof(tokenValidator), tokenValidator);
            _configuration = Guard.NotNull(nameof(configuration), configuration);
            _openIdConnectConfigurationReader = Guard.NotNull(nameof(openIdConnectConfigurationReader), openIdConnectConfigurationReader);
            _log = loggerFactory.CreateLogger<AzureADJwtBearerValidation>();
 
            var instance = _configuration["AzureADInstance"];
            var tenantId = _configuration["AzureADTenantId"];
            _wellKnownEndpoint = $"{instance}{tenantId}/v2.0/.well-known/openid-configuration";
            _audience = _configuration["ApiApplicationId"];
        }
 
        public async Task<bool> ValidateTokenAsync(string authorizationHeader)
        {
            _log.LogTrace($"{nameof(AzureADJwtBearerValidation)}.{nameof(ValidateTokenAsync)} called");
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
 
            var oidcWellknownEndpoints = await _openIdConnectConfigurationReader.GetConfigurationAsync(_wellKnownEndpoint);
  
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
                
                _log.LogTrace("Validating token");
                _tokenValidator.ValidateToken(accessToken, validationParameters, out securityToken);
                _log.LogTrace("Token validation succeeded");

                return true;
            }
            catch (Exception ex)
            {
                _log.LogDebug($"Token validation failed due to {ex.ToString()}");
            }
            return false;
        }
    }
}