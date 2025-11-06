using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayingGuest.Application.DTOs.Menus;
using PayingGuest.Application.Queries.Menus;
using PayingGuest.Common.Models;

namespace PayingGuest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMediator mediator, ILogger<MenuController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get menus for a specific user based on their roles
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<List<MenuDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserMenus(int userId)
        {
            var query = new GetUserMenusQuery { UserId = userId };
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get menus assigned to a specific role
        /// </summary>
        [HttpGet("role/{roleId}")]
        [ProducesResponseType(typeof(ApiResponse<List<MenuDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMenusByRole(int roleId)
        {
            var query = new GetMenusByRoleQuery { RoleId = roleId };
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get current user's menus
        /// </summary>
        [HttpGet("my-menus")]
        [ProducesResponseType(typeof(ApiResponse<List<MenuDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyMenus()
        {
            // Get user ID from JWT claims
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { Message = "Invalid user token" });
            }

            var query = new GetUserMenusQuery { UserId = userId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
