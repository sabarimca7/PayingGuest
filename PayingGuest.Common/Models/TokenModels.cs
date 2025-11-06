using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Common.Models
{
    public class TokenRequest
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string GrantType { get; set; } = "client_credentials";
        public string? Scope { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Code { get; set; }
        public string? RedirectUri { get; set; }
        public string? RefreshToken { get; set; }

    }
    public class TokenValidationRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class TokenValidationResponse
    {
        public bool Valid { get; set; }
        public string? Subject { get; set; }
        public string? ClientId { get; set; }
        public string[]? Scopes { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Error { get; set; }
    }

    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? EmailAddress { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class TokenResponse
    {
        public bool IsValid { get; set; }
        public int UserId { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public string? Scope { get; set; }
        
    }
}
