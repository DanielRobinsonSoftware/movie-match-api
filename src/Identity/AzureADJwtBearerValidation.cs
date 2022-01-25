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
        private ILoggerFactory _loggerFactory;
        private IOpenIdConnectConfigurationReader _openIdConnectConfigurationReader;

        public AzureADJwtBearerValidation(
            ISecurityTokenValidator tokenValidator,
            IConfiguration configuration,
            IOpenIdConnectConfigurationReader openIdConnectConfigurationReader,
            ILoggerFactory loggerFactory)
        {
            _tokenValidator = tokenValidator;
            _configuration = configuration;
            _openIdConnectConfigurationReader = openIdConnectConfigurationReader;
            _loggerFactory = loggerFactory;
        }
 
        public async Task<bool> ValidateTokenAsync(string authorizationHeader)
        {
            var wellKnownEndpoint = $"{_configuration["AzureADInstance"]}{_configuration["AzureADTenantId"]}/v2.0/.well-known/openid-configuration";
            var audience = _configuration["ApiApplicationId"];

            var log = _loggerFactory.CreateLogger<AzureADJwtBearerValidation>();
            log.LogTrace($"{nameof(AzureADJwtBearerValidation)}.{nameof(ValidateTokenAsync)} called");
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                log.LogDebug("Token validation failed due to missing authorization header");
                return false;
            }
 
            if (!authorizationHeader.Contains("Bearer"))
            {
                log.LogDebug("Token validation failed due to missing Bearer token");
                return false;
            }
 
            var accessToken = authorizationHeader.Substring("Bearer ".Length);
 
            var oidcWellknownEndpoints = await _openIdConnectConfigurationReader.GetConfigurationAsync(wellKnownEndpoint);
  
            var validationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidAudience = audience,
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
                
                log.LogTrace("Validating token");
                _tokenValidator.ValidateToken(accessToken, validationParameters, out securityToken);
                log.LogTrace("Token validation succeeded");

                return true;
            }
            catch (Exception ex)
            {
                log.LogDebug($"Token validation failed due to {ex.ToString()}");
            }
            return false;
        }
    }
}