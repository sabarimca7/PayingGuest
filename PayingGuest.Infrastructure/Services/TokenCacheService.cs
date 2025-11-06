using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.Interfaces;
using PayingGuest.Common.Models;

namespace PayingGuest.Infrastructure.Services
{
    public class TokenCacheService : ITokenCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<TokenCacheService> _logger;
        private const string TOKEN_KEY_PREFIX = "token_";
        private const string EXPIRY_KEY_PREFIX = "token_expiry_";

        public TokenCacheService(IMemoryCache cache, ILogger<TokenCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<TokenResponse?> GetTokenAsync(string clientId)
        {
            var key = $"{TOKEN_KEY_PREFIX}{clientId}";

            if (_cache.TryGetValue(key, out string? tokenJson) && !string.IsNullOrEmpty(tokenJson))
            {
                try
                {
                    var token = JsonSerializer.Deserialize<TokenResponse>(tokenJson);
                    return Task.FromResult(token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deserializing cached token");
                }
            }

            return Task.FromResult<TokenResponse?>(null);
        }

        public Task SetTokenAsync(string clientId, TokenResponse token)
        {
            var tokenKey = $"{TOKEN_KEY_PREFIX}{clientId}";
            var expiryKey = $"{EXPIRY_KEY_PREFIX}{clientId}";

            // Calculate absolute expiry time (expire 1 minute before actual expiry for safety)
            var expiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresIn - 60);
            var cacheExpiry = TimeSpan.FromSeconds(token.ExpiresIn - 60);

            var tokenJson = JsonSerializer.Serialize(token);

            // Store token and expiry time
            _cache.Set(tokenKey, tokenJson, cacheExpiry);
            _cache.Set(expiryKey, expiresAt, cacheExpiry);

            _logger.LogInformation("Token cached for client {ClientId}, expires at {ExpiresAt}", clientId, expiresAt);

            return Task.CompletedTask;
        }

        public Task RemoveTokenAsync(string clientId)
        {
            var tokenKey = $"{TOKEN_KEY_PREFIX}{clientId}";
            var expiryKey = $"{EXPIRY_KEY_PREFIX}{clientId}";

            _cache.Remove(tokenKey);
            _cache.Remove(expiryKey);

            _logger.LogInformation("Token removed from cache for client {ClientId}", clientId);

            return Task.CompletedTask;
        }

        public Task<bool> IsTokenExpiredAsync(string clientId)
        {
            var expiryKey = $"{EXPIRY_KEY_PREFIX}{clientId}";

            if (_cache.TryGetValue(expiryKey, out DateTime expiresAt))
            {
                return Task.FromResult(DateTime.UtcNow >= expiresAt);
            }

            return Task.FromResult(true); // Consider expired if not found
        }
    }
}