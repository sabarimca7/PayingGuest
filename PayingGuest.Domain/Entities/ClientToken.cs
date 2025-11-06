using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class ClientToken
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; }
        public string? RefreshToken { get; set; }
        public string? Scope { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
