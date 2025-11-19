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

            _identityServerUrl = configuration["IdentityServer:BaseUrl"] ?? throw new ArgumentNullException("IdentityServer:BaseUrl");
            _clientId = configuration["IdentityServer:ClientId"] ?? throw new ArgumentNullException("IdentityServer:ClientId");
            _clientSecret = configuration["IdentityServer:ClientSecret"] ?? throw new ArgumentNullException("IdentityServer:ClientSecret");
        }

        public async Task<TokenResponse> GetClientTokenAsync()
        {
            try
            {
                var request = new TokenRequest
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret,
                    GrantType = "client_credentials",
                    Scope = "read write",
                    Username = "",
                    Password = "",
                    Code = "",
                    RedirectUri = "",
                    RefreshToken = ""
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //Console.WriteLine(json);

                var response = await _httpClient.PostAsJsonAsync($"{_identityServerUrl}/api/Token/token", request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to get token: {Error}", error);
                    throw new TokenException($"Failed to get token: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tokenResponse == null)
                {
                    throw new TokenException("Invalid token response");
                }

                // Store token in cache
                await _tokenCache.SetTokenAsync(_clientId, tokenResponse);

                // Store token in database
                await StoreTokenInDatabase(tokenResponse);

                return tokenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client token");
                throw new TokenException($"Failed to obtain access token:{ex}");
            }
        }

        public async Task<TokenValidationResponse> ValidateTokenAsync(string token)
        {
            try
            {
                var request = new TokenValidationRequest { Token = token };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_identityServerUrl}/api/Token/validate", content);

                if (!response.IsSuccessStatusCode)
                {
                    return new TokenValidationResponse
                    {
                        Valid = false,
                        Error = "Token validation failed"
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var validationResponse = JsonSerializer.Deserialize<TokenValidationResponse>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return validationResponse ?? new TokenValidationResponse { Valid = false };
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
            var validationResponse = await ValidateTokenAsync(token);
            return validationResponse.Valid;
        }

        public async Task<string> GetOrRefreshTokenAsync()
        {
            // Check cache first
            var cachedToken = await _tokenCache.GetTokenAsync(_clientId);

            if (cachedToken != null && !await _tokenCache.IsTokenExpiredAsync(_clientId))
            {
                return cachedToken.AccessToken;
            }

            // Get new token
            var tokenResponse = await GetClientTokenAsync();
            return tokenResponse.AccessToken;
        }

        private async Task StoreTokenInDatabase(TokenResponse tokenResponse)
        {
            try
            {
                // Deactivate old tokens
                var existingTokens = await _unitOfWork.ClientTokens.FindAsync(t => t.ClientId == _clientId && t.IsActive);
                foreach (var token in existingTokens)
                {
                    token.IsActive = false;
                    await _unitOfWork.ClientTokens.UpdateAsync(token);
                }

                // Store new token
                var clientToken = new ClientToken
                {
                    ClientId = _clientId,
                    AccessToken = tokenResponse.AccessToken,
                    TokenType = tokenResponse.TokenType,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    RefreshToken = tokenResponse.RefreshToken,
                    Scope = tokenResponse.Scope,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };

                await _unitOfWork.ClientTokens.AddAsync(clientToken);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing token in database");
            }
        }

        public async Task<int> CreateUserAsync(string username, string password, string firstName, string lastName)
        {
            try
            {
                // Get client token first
                var tokenResponse = await GetClientTokenAsync();
                if (string.IsNullOrEmpty(tokenResponse?.AccessToken))
                {
                    throw new TokenException("Failed to obtain client token");
                }
                var request = new CreateUserDto
                {
                    Username = username,
                    Password = password,
                    Firstname = firstName,
                    Lastname = lastName
                };

                _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

                var response = await _httpClient.PostAsJsonAsync($"{_identityServerUrl}/api/User", request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new TokenException($"Failed to get user: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var userResponse = JsonSerializer.Deserialize<ApiResponse<UserDto>>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (userResponse == null)
                {
                    throw new TokenException("Invalid user response");
                }
                return userResponse.Data.UserId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client token");
                throw new TokenException($"Failed to obtain access token:{ex}");
            }
        }
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
                    Password = password,
                    Code = "",
                    RedirectUri = "",
                    RefreshToken = ""
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //Console.WriteLine(json);

                var response = await _httpClient.PostAsJsonAsync($"{_identityServerUrl}/api/Token/token", request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new TokenException($"Failed to get user: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var userResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (userResponse == null)
                {
                    throw new TokenException("Invalid user response");
                }
                return userResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting client token");
                throw new TokenException($"Failed to obtain access token:{ex}");
            }
        }

        public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var refreshTokenEndpoint = $"{_identityServerUrl}/api/Token/refresh";

                var requestBody = new Dictionary<string, string>
                                    {
                                        { "grant_type", "refresh_token" },
                                        { "client_id", _clientId! },
                                        { "client_secret", _clientSecret! },
                                        { "refresh_token", refreshToken }
                                    };

                var request = new HttpRequestMessage(HttpMethod.Post, refreshTokenEndpoint)
                {
                    Content = new FormUrlEncodedContent(requestBody)
                };

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return null;
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return tokenResponse;
            }
            catch (Exception)
            {
                // Log exception if you have logging configured
                return null;
            }
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            try
            {
                var revocationEndpoint = $"{_identityServerUrl}/api/Token/revoke";

                if (string.IsNullOrEmpty(revocationEndpoint))
                {
                    return true;
                }

                var requestBody = new Dictionary<string, string>
                        {
                            { "token", token },
                            { "client_id", _clientSecret! },
                            { "client_secret", _clientSecret! },
                            { "token_type_hint", "refresh_token" }
                        };

                var request = new HttpRequestMessage(HttpMethod.Post, revocationEndpoint)
                {
                    Content = new FormUrlEncodedContent(requestBody)
                };

                var response = await _httpClient.SendAsync(request);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}