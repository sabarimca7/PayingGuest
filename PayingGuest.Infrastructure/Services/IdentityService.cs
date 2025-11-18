using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Common.Exceptions;
using PayingGuest.Common.Models;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ITokenCacheService _tokenCache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IdentityService> _logger;

        private readonly string _identityServerUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public IdentityService(
            HttpClient httpClient,
            IConfiguration configuration,
            ITokenCacheService tokenCache,
            IUnitOfWork unitOfWork,
            ILogger<IdentityService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _tokenCache = tokenCache;
            _unitOfWork = unitOfWork;
            _logger = logger;

            _identityServerUrl = configuration["IdentityServer:BaseUrl"]
                ?? throw new ArgumentNullException("IdentityServer:BaseUrl");

            _clientId = configuration["IdentityServer:ClientId"]
                ?? throw new ArgumentNullException("IdentityServer:ClientId");

            _clientSecret = configuration["IdentityServer:ClientSecret"]
                ?? throw new ArgumentNullException("IdentityServer:ClientSecret");
        }

        // ======================================================
        // 1. GET CLIENT TOKEN
        // ======================================================
        public async Task<TokenResponse> GetClientTokenAsync()
        {
            try
            {
                var request = new TokenRequest
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "client_credentials",
                    Scope = "read write"
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{_identityServerUrl}/api/Token/token", request);

                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to get client token: {Error}", err);
                    throw new TokenException($"Failed to get token: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? throw new TokenException("Invalid token response");

                await _tokenCache.SetTokenAsync(_clientId, tokenResponse);
                await StoreTokenInDatabase(tokenResponse);

                return tokenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client token");
                throw new TokenException($"Failed to obtain access token: {ex.Message}");
            }
        }

        // ======================================================
        // 2. VALIDATE TOKEN
        // ======================================================
        public async Task<TokenValidationResponse> ValidateTokenAsync(string token)
        {
            try
            {
                var body = new TokenValidationRequest { Token = token };
                var content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(
                    $"{_identityServerUrl}/api/Token/validate",
                    content);

                if (!response.IsSuccessStatusCode)
                {
                    return new TokenValidationResponse
                    {
                        Valid = false,
                        Error = "Token validation failed"
                    };
                }

                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<TokenValidationResponse>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new TokenValidationResponse { Valid = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return new TokenValidationResponse
                {
                    Valid = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var result = await ValidateTokenAsync(token);
            return result?.Valid ?? false;
        }

        // ======================================================
        // 3. GET OR REFRESH TOKEN
        // ======================================================
        public async Task<string> GetOrRefreshTokenAsync()
        {
            var cached = await _tokenCache.GetTokenAsync(_clientId);

            if (cached != null && !await _tokenCache.IsTokenExpiredAsync(_clientId))
            {
                return cached.AccessToken ?? "";
            }

            var newToken = await GetClientTokenAsync();
            return newToken.AccessToken ?? "";
        }


        // ======================================================
        // 4. STORE TOKEN IN DATABASE
        // ======================================================
        private async Task StoreTokenInDatabase(TokenResponse token)
        {
            try
            {
                var oldTokens = await _unitOfWork.ClientTokens
                    .FindAsync(t => t.ClientId == _clientId && t.IsActive);

                foreach (var t in oldTokens)
                {
                    t.IsActive = false;
                    await _unitOfWork.ClientTokens.UpdateAsync(t);
                }

                var entity = new ClientToken
                {
                    ClientId = _clientId,
                    AccessToken = token.AccessToken,
                    TokenType = token.TokenType,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresIn),
                    RefreshToken = token.RefreshToken,
                    Scope = token.Scope,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                await _unitOfWork.ClientTokens.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store token in database");
            }
        }

        // ======================================================
        // 5. CREATE USER (calls Identity Server)
        // ======================================================
        public async Task<int> CreateUserAsync(string username, string password, string firstName, string lastName)
        {
            try
            {
                var token = await GetClientTokenAsync();

                if (string.IsNullOrWhiteSpace(token.AccessToken))
                    throw new TokenException("Failed to obtain client token");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

                var request = new CreateUserDto
                {
                    Username = username,
                    Password = password,
                    Firstname = firstName,
                    Lastname = lastName
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{_identityServerUrl}/api/User",
                    request);

                if (!response.IsSuccessStatusCode)
                    throw new TokenException($"Failed to create user: {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();
                var userResponse = JsonSerializer.Deserialize<ApiResponse<UserDto>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? throw new TokenException("Invalid user response");

                return userResponse.Data?.UserId
                    ?? throw new TokenException("User ID missing in response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw new TokenException($"Failed to create user: {ex.Message}");
            }
        }

        // ======================================================
        // 6. VALIDATE CREDENTIALS
        // ======================================================
        public async Task<TokenResponse?> ValidateCredentialsAsync(string username, string password)
        {
            try
            {
                var request = new TokenRequest
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "password",
                    Scope = "read write",
                    Username = username,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{_identityServerUrl}/api/Token/token",
                    request);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<TokenResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating credentials");
                return null;
            }
        }

        // ======================================================
        // 7. REFRESH TOKEN
        // ======================================================
        public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var url = $"{_identityServerUrl}/api/Token/refresh";

                var dict = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "refresh_token", refreshToken }
                };

                var response = await _httpClient.PostAsync(
                    url,
                    new FormUrlEncodedContent(dict));

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<TokenResponse>();
            }
            catch
            {
                return null;
            }
        }

        // ======================================================
        // 8. REVOKE TOKEN
        // ======================================================
        public async Task<bool> RevokeTokenAsync(string token)
        {
            try
            {
                var url = $"{_identityServerUrl}/api/Token/revoke";

                var dict = new Dictionary<string, string>
                {
                    { "token", token },
                    { "client_id", _clientId },      // FIXED (was wrong before)
                    { "client_secret", _clientSecret },
                    { "token_type_hint", "refresh_token" }
                };

                var response = await _httpClient.PostAsync(
                    url,
                    new FormUrlEncodedContent(dict));

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
