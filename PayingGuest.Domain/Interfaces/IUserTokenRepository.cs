using PayingGuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Interfaces
{
    public interface IUserTokenRepository : IRepository<UserToken>
    {
        Task<UserToken?> GetByRefreshTokenAsync(string refreshToken);
        Task<UserToken?> GetActiveTokenByUserIdAsync(int userId);
        Task<List<UserToken>> GetUserTokensAsync(int userId);
        Task RevokeTokenAsync(string refreshToken, string? revokedByIp = null);
        Task RevokeAllUserTokensAsync(int userId);
        Task<bool> IsRefreshTokenValidAsync(string refreshToken);
        Task CleanupExpiredTokensAsync();

    }
}
