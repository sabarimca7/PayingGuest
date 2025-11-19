using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.DTOs.Auth;
using PayingGuest.Application.Interfaces;
using PayingGuest.Common.Models;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands.Auth
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ApiResponse<LoginResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LoginUserCommandHandler> _logger;

        public LoginUserCommandHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<LoginUserCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<LoginResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate credentials with IdentityServer
                var tokenResponse = await _identityService.ValidateCredentialsAsync(
                    request.Username,
                    request.Password);

                if (string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password");
                }

                // Get user details from PayingGuest database
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Username);
                if (user == null || !user.IsActive)
                {
                    return ApiResponse<LoginResponseDto>.ErrorResponse("User not found or inactive");
                }
                // Get IP address and device info
                var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

                // Save token to database
                await _tokenService.SaveUserTokenAsync(
                    user.UserId,
                    tokenResponse,
                    ipAddress,
                    userAgent);


                // Get user with roles
                var userWithRoles = await _unitOfWork.Users.GetUserWithRolesAsync(user.UserId);
                var roleIds = userWithRoles?.UserRoles
                    .Where(ur => ur.IsActive)
                    .Select(ur => ur.RoleId)
                    .ToList() ?? new List<int>();

                // Get user menus based on roles
                var menus = await _unitOfWork.Menus.GetMenusByRoleIdsAsync(roleIds);

                var response = new LoginResponseDto
                {
                    AccessToken = tokenResponse.AccessToken,
                    TokenType = tokenResponse.TokenType,
                    ExpiresIn = tokenResponse.ExpiresIn,
                    RefreshToken = tokenResponse.RefreshToken,
                    User = new UserInfoDto
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        EmailAddress = user.EmailAddress,
                        Roles = userWithRoles?.UserRoles
                            .Where(ur => ur.IsActive)
                            .Select(ur => ur.Role.RoleName ?? "")
                            .ToList() ?? new List<string>()
                    },
                    Menus = menus.Select(m => new MenuItemDto
                    {
                        MenuId = m.MenuId,
                        MenuName = m.MenuName,
                        MenuUrl = m.MenuUrl,
                        Icon = m.MenuIcon,
                        ParentMenuId = m.ParentMenuId,
                        DisplayOrder = m.DisplayOrder
                    }).ToList()
                };

                return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error during login for user {Email}", request.Username);
                return ApiResponse<LoginResponseDto>.ErrorResponse($"Login failed: {ex.Message}");
            }
        }
    }

}
