using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PayingGuest.Application.Interfaces;
using PayingGuest.Common.Models;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PayingGuest.Tests.Unit
{
    public class IdentityServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ITokenCacheService> _tokenCacheMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<IdentityService>> _loggerMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly IdentityService _identityService;

        public IdentityServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _tokenCacheMock = new Mock<ITokenCacheService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<IdentityService>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _configurationMock.Setup(x => x["IdentityServer:BaseUrl"])
                .Returns("https://localhost:5001");
            _configurationMock.Setup(x => x["IdentityServer:ClientId"])
                .Returns("client_web_app_001");
            _configurationMock.Setup(x => x["IdentityServer:ClientSecret"])
                .Returns("secret_web_001_AbCdEfGhIjKlMnOpQrStUvWxYz");

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://localhost:5001")
            };

            _identityService = new IdentityService(
                _httpClient,
                _configurationMock.Object,
                _tokenCacheMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GetClientTokenAsync_ShouldReturnToken_WhenRequestSuccessful()
        {
            // Arrange
            var tokenResponse = new TokenResponse
            {
                AccessToken = "test_access_token",
                TokenType = "Bearer",
                ExpiresIn = 3600,
                Scope = "read write"
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(tokenResponse))
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            _unitOfWorkMock.Setup(x => x.ClientTokens.FindAsync(It.IsAny<Expression<Func<Domain.Entities.ClientToken, bool>>>()))
                .ReturnsAsync(new List<Domain.Entities.ClientToken>());

            _unitOfWorkMock.Setup(x => x.ClientTokens.AddAsync(It.IsAny<Domain.Entities.ClientToken>()))
                .ReturnsAsync(new Domain.Entities.ClientToken());

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _identityService.GetClientTokenAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test_access_token", result.AccessToken);
            Assert.Equal(3600, result.ExpiresIn);
            _tokenCacheMock.Verify(x => x.SetTokenAsync(It.IsAny<string>(), It.IsAny<TokenResponse>()), Times.Once);
        }

        [Fact]
        public async Task ValidateTokenAsync_ShouldReturnValid_WhenTokenIsValid()
        {
            // Arrange
            var validationResponse = new TokenValidationResponse
            {
                Valid = true,
                ClientId = "client_web_app_001",
                Subject = "test_user",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(validationResponse))
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _identityService.ValidateTokenAsync("test_token");

            // Assert
            Assert.True(result.Valid);
            Assert.Equal("client_web_app_001", result.ClientId);
            Assert.Equal("test_user", result.Subject);
        }

        [Fact]
        public async Task GetOrRefreshTokenAsync_ShouldReturnCachedToken_WhenNotExpired()
        {
            // Arrange
            var cachedToken = new TokenResponse
            {
                AccessToken = "cached_token",
                TokenType = "Bearer",
                ExpiresIn = 3600
            };

            _tokenCacheMock.Setup(x => x.GetTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(cachedToken);

            _tokenCacheMock.Setup(x => x.IsTokenExpiredAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var result = await _identityService.GetOrRefreshTokenAsync();

            // Assert
            Assert.Equal("cached_token", result);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}