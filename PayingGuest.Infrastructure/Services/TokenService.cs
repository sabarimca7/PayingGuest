using Microsoft.Extensions.Logging;
using PayingGuest.Application.Interfaces;
using PayingGuest.Common.Models;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            ILogger<TokenService> logger)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<UserToken> SaveUserTokenAsync(
            int userId,
            TokenResponse tokenResponse,
            string? ipAddress = null,
            string? deviceInfo = null)
        {
            try
            {
                // Revoke any existing active tokens for the user
                await _unitOfWork.UserTokens.RevokeAllUserTokensAsync(userId);
                await _unitOfWork.SaveChangesAsync();

                // Create new token record
                var userToken = new UserToken
                {
                    UserId = userId,
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    TokenType = tokenResponse.TokenType,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    RefreshTokenExpiresAt = !string.IsNullOrEmpty(tokenResponse.RefreshToken)
                        ? DateTime.UtcNow.AddDays(7) // 7 days for refresh token
                        : null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByIp = ipAddress,
                    DeviceInfo = deviceInfo,
                    IsActive = true,
                    IsRevoked = false
                };

                await _unitOfWork.UserTokens.AddAsync(userToken);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Token saved for user {UserId}", userId);

                return userToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving token for user {UserId}", userId);
                throw;
            }
        }

        public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
        {
            try
            {
                var userToken = await _unitOfWork.UserTokens.GetByRefreshTokenAsync(refreshToken);

                if (userToken == null || !userToken.IsValidRefreshToken)
                {
                    _logger.LogWarning("Invalid refresh token attempt from IP: {IpAddress}", ipAddress);
                    return null;
                }

                // Get new token from IdentityServer using refresh token
                var newTokenResponse = await _identityService.RefreshTokenAsync(refreshToken);

                if (newTokenResponse == null || string.IsNullOrEmpty(newTokenResponse.AccessToken))
                {
                    return null;
                }

                // Revoke old token
                await _unitOfWork.UserTokens.RevokeTokenAsync(refreshToken, ipAddress);

                // Save new token
                var newUserToken = await SaveUserTokenAsync(
                    userToken.UserId,
                    newTokenResponse,
                    ipAddress,
                    userToken.DeviceInfo);

                // Update the replaced token reference
                userToken.ReplacedByToken = newUserToken.RefreshToken;
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Token refreshed for user {UserId}", userToken.UserId);

                return newTokenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return null;
            }
        }

        public async Task RevokeTokenAsync(string refreshToken, string? ipAddress = null)
        {
            await _unitOfWork.UserTokens.RevokeTokenAsync(refreshToken, ipAddress);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Token revoked from IP: {IpAddress}", ipAddress);
        }

        public async Task RevokeAllUserTokensAsync(int userId)
        {
            await _unitOfWork.UserTokens.RevokeAllUserTokensAsync(userId);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("All tokens revoked for user {UserId}", userId);
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            return await _unitOfWork.UserTokens.IsRefreshTokenValidAsync(refreshToken);
        }

        public async Task CleanupExpiredTokensAsync()
        {
            await _unitOfWork.UserTokens.CleanupExpiredTokensAsync();
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Expired tokens cleaned up");
        }
    }
}
