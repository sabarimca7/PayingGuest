using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayingGuest.Application.Commands;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Queries;

namespace PayingGuest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var result = await _mediator.Send(new GetProfileQuery { UserId = userId });

            return Ok(result);
        }
        // ✅ PUT: api/profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile( [FromBody] UpdateProfileDto dto)
        {
            await _mediator.Send(new UpdateProfileCommand
            {
                Profile = dto
            });

            return Ok(new
            {
                message = "Profile updated successfully"
            });
        }
    }
}

