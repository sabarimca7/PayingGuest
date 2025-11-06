using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using PayingGuest.Common.Models;
using PayingGuest.Infrastructure.Services;
using Xunit;

namespace PayingGuest.Tests.Unit
{
    public class TokenCacheServiceTests
    {
        private readonly IMemoryCache _cache;
        private readonly Mock<ILogger<TokenCacheService>> _loggerMock;
        private readonly TokenCacheService _tokenCacheService;

        public TokenCacheServiceTests()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _loggerMock = new Mock<ILogger<TokenCacheService>>();
            _tokenCacheService = new TokenCacheService(_cache, _loggerMock.Object);
        }

        [Fact]
        public async Task SetTokenAsync_ShouldStoreTokenInCache()
        {
            // Arrange
            var token = new TokenResponse
            {
                AccessToken = "test_token",
                TokenType = "Bearer",
                ExpiresIn = 3600
            };
            var clientId = "test_client";

            // Act
            await _tokenCacheService.SetTokenAsync(clientId, token);

            // Assert
            var retrievedToken = await _tokenCacheService.GetTokenAsync(clientId);
            Assert.NotNull(retrievedToken);
            Assert.Equal("test_token", retrievedToken.AccessToken);
        }

        [Fact]
        public async Task GetTokenAsync_ShouldReturnNull_WhenTokenNotInCache()
        {
            // Act
            var result = await _tokenCacheService.GetTokenAsync("non_existent_client");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveTokenAsync_ShouldRemoveTokenFromCache()
        {
            // Arrange
            var token = new TokenResponse
            {
                AccessToken = "test_token",
                TokenType = "Bearer",
                ExpiresIn = 3600
            };
            var clientId = "test_client";

            await _tokenCacheService.SetTokenAsync(clientId, token);

            // Act
            await _tokenCacheService.RemoveTokenAsync(clientId);

            // Assert
            var result = await _tokenCacheService.GetTokenAsync(clientId);
            Assert.Null(result);
        }

        [Fact]
        public async Task IsTokenExpiredAsync_ShouldReturnTrue_WhenTokenNotFound()
        {
            // Act
            var result = await _tokenCacheService.IsTokenExpiredAsync("non_existent_client");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsTokenExpiredAsync_ShouldReturnFalse_WhenTokenNotExpired()
        {
            // Arrange
            var token = new TokenResponse
            {
                AccessToken = "test_token",
                TokenType = "Bearer",
                ExpiresIn = 3600 // 1 hour
            };
            var clientId = "test_client";

            // Act
            await _tokenCacheService.SetTokenAsync(clientId, token);
            var result = await _tokenCacheService.IsTokenExpiredAsync(clientId);

            // Assert
            Assert.False(result);
        }
    }
}