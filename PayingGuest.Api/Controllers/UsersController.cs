using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayingGuest.Application.Commands;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Queries;
using PayingGuest.Common.Models;

namespace PayingGuest.Api.Controllers
{
   // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        ///// <summary>
        ///// Register a new user
        ///// </summary>
        ///// <param name="registerUserDto">User registration details</param>
        ///// <returns>Created user details</returns>
        //[HttpPost("register")]
        //[ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        //{
        //    _logger.LogInformation("Registering new user with email: {Email}", registerUserDto.EmailAddress);

        //    var command = new RegisterUserCommand { RegisterUserDto = registerUserDto };
        //    var result = await _mediator.Send(command);

        //    if (result.Success)
        //    {
        //        return CreatedAtAction(nameof(GetUser), new { id = result.Data?.UserId }, result);
        //    }

        //    return BadRequest(result);
        //}

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(int id)
        {
            _logger.LogInformation("Getting user with ID: {UserId}", id);

            var query = new GetUserQuery { UserId = id };
            var result = await _mediator.Send(query);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// Get users by property and type
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="userType">User type (Admin/Manager/Guest/Staff)</param>
        /// <returns>List of users</returns>
        [HttpGet("property/{propertyId}/type/{userType}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersByType(int propertyId, string userType)
        {
            _logger.LogInformation("Getting users for property {PropertyId} with type {UserType}", propertyId, userType);

            var query = new GetUsersByTypeQuery
            {
                PropertyId = propertyId,
                UserType = userType
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserDto">Updated user details</param>
        /// <returns>Updated user details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Updating user with ID: {UserId}", id);

            var command = new UpdateUserCommand
            {
                UserId = id,
                UpdateUserDto = updateUserDto
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        /// <summary>
        /// Delete user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);

            var command = new DeleteUserCommand { UserId = id };
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

    }
}