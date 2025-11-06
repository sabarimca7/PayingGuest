using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.Interfaces;
using PayingGuest.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands.Auth.Refresh
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<TokenResponse>>
    {
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResponse<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

                var newToken = await _tokenService.RefreshTokenAsync(request.RefreshToken, ipAddress);

                if (newToken == null)
                {
                    return ApiResponse<TokenResponse>.ErrorResponse("Invalid or expired refresh token");
                }

                return ApiResponse<TokenResponse>.SuccessResponse(newToken, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, "Error refreshing token");
                return ApiResponse<TokenResponse>.ErrorResponse("Failed to refresh token");
            }
        }
    }
}
