using System.Threading.Tasks;
using PayingGuest.Common.Models;

namespace PayingGuest.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<TokenResponse> GetClientTokenAsync();
        Task<TokenValidationResponse> ValidateTokenAsync(string token);
        Task<bool> IsTokenValidAsync(string token);
        Task<string> GetOrRefreshTokenAsync();
        Task<int> CreateUserAsync(string username, string password, string firstName, string lastName);
        Task<TokenResponse?> ValidateCredentialsAsync(string username, string password);
        Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string token);
    }
}