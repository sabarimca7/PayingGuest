using PayingGuest.Common.Models;
using PayingGuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Interfaces
{
    public interface ITokenService
    {
        Task<UserToken> SaveUserTokenAsync(int userId, TokenResponse tokenResponse, string? ipAddress = null, string? deviceInfo = null);
        Task<TokenResponse?> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
        Task RevokeTokenAsync(string refreshToken, string? ipAddress = null);
        Task RevokeAllUserTokensAsync(int userId);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);
        Task CleanupExpiredTokensAsync();
    }
}
