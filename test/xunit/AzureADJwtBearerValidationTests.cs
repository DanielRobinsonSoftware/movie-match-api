using Xunit;
using NSubstitute;
using MovieMatch.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace movie_match_api.tests;

public class AzureADJwtBearerValidationTests
{
    AzureADJwtBearerValidation _jwtValidator;
    IJwtSecurityTokenHandler _tokenValidator;

    public AzureADJwtBearerValidationTests()
    {
        _tokenValidator = Substitute.For<IJwtSecurityTokenHandler>();
        _tokenValidator.ValidateToken(Arg.Any<string>(), Arg.Any<TokenValidationParameters>(), out Arg.Any<SecurityToken>());

        var configuration = Substitute.For<IConfiguration>();

        var openIdConnectConfigurationReader = Substitute.For<IOpenIdConnectConfigurationReader>();
        openIdConnectConfigurationReader.GetConfigurationAsync(Arg.Any<string>())
            .Returns(new OpenIdConnectConfiguration());

        var loggerFactory = Substitute.For<ILoggerFactory>();

        _jwtValidator = new AzureADJwtBearerValidation(_tokenValidator, configuration, openIdConnectConfigurationReader, loggerFactory);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Given_Authorization_header_is_missing_when_ValidateTokenAsync_is_called_then_return_false(string authorizationHeader)
    {
        // Act
        var validationResult = _jwtValidator.ValidateTokenAsync(authorizationHeader);

        // Assert
        validationResult.Result.Should().BeFalse();
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("Invalid token")]
    public void Given_Authorization_header_does_not_contain_Bearer_token_when_ValidateTokenAsync_is_called_then_return_false(string authorizationHeader)
    {
        // Act
        var validationResult = _jwtValidator.ValidateTokenAsync(authorizationHeader);

        // Assert
        validationResult.Result.Should().BeFalse();
    }

    [Fact]
    public void Given_Authorization_header_contains_invalid_Bearer_token_when_ValidateTokenAsync_is_called_then_catch_exception_and_return_false()
    {
        const string expectedToken = "InvalidJWT";
        var authorizationHeader = $"Bearer {expectedToken}";
        _tokenValidator
            .When(x => x.ValidateToken(Arg.Is<string>(expectedToken), Arg.Any<TokenValidationParameters>(), out Arg.Any<SecurityToken>()))
            .Do(x => { throw new SecurityTokenValidationException(); });

        // Act
        var validationResult = _jwtValidator.ValidateTokenAsync(authorizationHeader);

        // Assert
        _tokenValidator.Received().ValidateToken(Arg.Is<string>(expectedToken), Arg.Any<TokenValidationParameters>(), out Arg.Any<SecurityToken>());
        validationResult.Result.Should().BeFalse();
    }

    [Fact]
    public void Given_Authorization_header_contains_valid_Bearer_token_when_ValidateTokenAsync_is_called_then_return_true()
    {
        const string expectedToken = "FakeJWT";
        var authorizationHeader = $"Bearer {expectedToken}";

        // Act
        var validationResult = _jwtValidator.ValidateTokenAsync(authorizationHeader);

        // Assert
        _tokenValidator.Received().ValidateToken(Arg.Is<string>(expectedToken), Arg.Any<TokenValidationParameters>(), out Arg.Any<SecurityToken>());
        validationResult.Result.Should().BeTrue();
    }
}