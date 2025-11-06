using Microsoft.EntityFrameworkCore;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Repositories
{
    public class UserTokenRepository : Repository<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(PayingGuestDbContext context) : base(context)
        {
        }

        public async Task<UserToken?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbSet
                .Include(ut => ut.User)
                .FirstOrDefaultAsync(ut => ut.RefreshToken == refreshToken && ut.IsActive);
        }

        public async Task<UserToken?> GetActiveTokenByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(ut => ut.UserId == userId && ut.IsActive && !ut.IsRevoked)
                .OrderByDescending(ut => ut.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserToken>> GetUserTokensAsync(int userId)
        {
            return await _dbSet
                .Where(ut => ut.UserId == userId)
                .OrderByDescending(ut => ut.CreatedAt)
                .ToListAsync();
        }

        public async Task RevokeTokenAsync(string refreshToken, string? revokedByIp = null)
        {
            var token = await GetByRefreshTokenAsync(refreshToken);
            if (token != null)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = revokedByIp;
                _dbSet.Update(token);
            }
        }

        public async Task RevokeAllUserTokensAsync(int userId)
        {
            var tokens = await _dbSet
                .Where(ut => ut.UserId == userId && ut.IsActive && !ut.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }
        }

        public async Task<bool> IsRefreshTokenValidAsync(string refreshToken)
        {
            var token = await GetByRefreshTokenAsync(refreshToken);
            return token != null && token.IsValidRefreshToken;
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = await _dbSet
                .Where(ut => ut.ExpiresAt < DateTime.UtcNow ||
                            (ut.RefreshTokenExpiresAt.HasValue && ut.RefreshTokenExpiresAt.Value < DateTime.UtcNow))
                .ToListAsync();

            foreach (var token in expiredTokens)
            {
                token.IsActive = false;
            }
        }
    }
}
