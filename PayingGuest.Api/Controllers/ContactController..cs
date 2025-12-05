using MediatR;
using Microsoft.AspNetCore.Mvc;
using PayingGuest.Application.Commands.Contact;
using PayingGuest.Application.DTOs;

namespace PayingGuest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContactController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactMessageDto dto)
        {
            var result = await _mediator.Send(new CreateContactMessageCommand { Data = dto });

            if (result)
                return Ok(new { message = "Message sent successfully" });

            return BadRequest("Something went wrong!");
        }
    }
}
