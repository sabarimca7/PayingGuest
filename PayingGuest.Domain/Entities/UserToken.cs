using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class UserToken
    {
        public int UserTokenId { get; set; }
        public int UserId { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByIp { get; set; }
        public string? DeviceInfo { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual User User { get; set; } = null!;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRefreshTokenExpired => RefreshTokenExpiresAt.HasValue && DateTime.UtcNow >= RefreshTokenExpiresAt.Value;
        public bool IsValidRefreshToken => !string.IsNullOrEmpty(RefreshToken) && !IsRefreshTokenExpired && !IsRevoked && IsActive;
    }
}
