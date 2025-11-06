using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayingGuest.Application.Commands;
using PayingGuest.Application.Commands.Auth;
using PayingGuest.Application.Commands.Auth.Refresh;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.DTOs.Auth;
using PayingGuest.Application.Interfaces;
using PayingGuest.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace PayingGuest.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        public AuthController(IIdentityService identityService, 
            IMediator mediator, ILogger<AuthController> logger, ITokenService tokenService)
        {
            _identityService = identityService;
            _mediator = mediator;
            _logger = logger;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Get Client Token
        /// </summary>
        /// <returns>token details</returns>
        [HttpPost("authclienttoken")]
        public async Task<IActionResult> authclienttoken()
        {
            var result = await _identityService.GetClientTokenAsync();

            if (result.AccessToken is not null)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new RegisterUserCommand { RegisterUserDto = request };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Login user
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new LoginUserCommand
            {
                Username = request.Username,
                Password = request.Password
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Logout user (client-side token removal)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            // In JWT authentication, logout is typically handled client-side
            // by removing the token from storage
            _logger.LogInformation("User logged out");
            return Ok(new { Message = "Logged out successfully" });
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var command = new RefreshTokenCommand { RefreshToken = request.RefreshToken };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("revoke-token")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _tokenService.RevokeTokenAsync(request.RefreshToken, ipAddress);

            return Ok(new { Message = "Token revoked successfully" });
        }
    }
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
    public class RevokeTokenRequest
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}