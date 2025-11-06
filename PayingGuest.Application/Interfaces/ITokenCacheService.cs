using System.Threading.Tasks;
using PayingGuest.Common.Models;

namespace PayingGuest.Application.Interfaces
{
    public interface ITokenCacheService
    {
        Task<TokenResponse?> GetTokenAsync(string clientId);
        Task SetTokenAsync(string clientId, TokenResponse token);
        Task RemoveTokenAsync(string clientId);
        Task<bool> IsTokenExpiredAsync(string clientId);
    }
}