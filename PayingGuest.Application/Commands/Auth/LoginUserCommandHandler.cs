using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PayingGuest.Application.DTOs.Auth;
using PayingGuest.Application.Interfaces;
using PayingGuest.Common.Models;
using PayingGuest.Domain.Interfaces;

namespace PayingGuest.Application.Commands.Auth
{
    public class LoginUserCommandHandler :
        IRequestHandler<LoginUserCommand, ApiResponse<LoginResponseDto>>
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
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResponse<LoginResponseDto>> Handle(
            LoginUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // ------------------------------------------------------------------
                // 1. Validate credentials with IdentityServer
                // ------------------------------------------------------------------
                var tokenResponse = await _identityService.ValidateCredentialsAsync(
                    request.Username,
                    request.Password);

                if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                {
                    return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password.");
                }

                // ------------------------------------------------------------------
                // 2. Validate user from database
                // ------------------------------------------------------------------
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Username);

                if (user == null || !user.IsActive)
                {
                    return ApiResponse<LoginResponseDto>.ErrorResponse("User not found or inactive.");
                }

                // ------------------------------------------------------------------
                // 3. Get IP address & user-agent safely
                // ------------------------------------------------------------------
                var context = _httpContextAccessor.HttpContext;

                var ipAddress = context?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = context?.Request?.Headers["User-Agent"].ToString() ?? "unknown";

                // ------------------------------------------------------------------
                // 4. Save token in DB
                // ------------------------------------------------------------------
                await _tokenService.SaveUserTokenAsync(
                    user.UserId,
                    tokenResponse,
                    ipAddress,
                    userAgent);

                // ------------------------------------------------------------------
                // 5. Get user with roles
                // ------------------------------------------------------------------
                var userWithRoles = await _unitOfWork.Users.GetUserWithRolesAsync(user.UserId);

                var activeRoleIds = userWithRoles?.UserRoles?
                    .Where(r => r.IsActive)
                    .Select(r => r.RoleId)
                    .ToList()
                    ?? new List<int>();

                // ------------------------------------------------------------------
                // 6. Get menu items for these roles
                // ------------------------------------------------------------------
                var menus = await _unitOfWork.Menus.GetMenusByRoleIdsAsync(activeRoleIds)
                            ?? new List<Domain.Entities.Menu>();

                // ------------------------------------------------------------------
                // 7. Prepare response DTO
                // ------------------------------------------------------------------
                var response = new LoginResponseDto
                {
                    AccessToken = tokenResponse.AccessToken ?? "",
                    TokenType = tokenResponse.TokenType,
                    ExpiresIn = tokenResponse.ExpiresIn,
                    RefreshToken = tokenResponse.RefreshToken,
                    User = new UserInfoDto
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        EmailAddress = user.EmailAddress,
                        Roles = userWithRoles?.UserRoles?
                            .Where(r => r.IsActive)
                            .Select(r => r.Role?.RoleName ?? "")
                            .ToList()
                            ?? new List<string>()
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

                return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for user {Email}", request.Username);
                return ApiResponse<LoginResponseDto>.ErrorResponse($"Login failed: {ex.Message}");
            }
        }
    }
}
