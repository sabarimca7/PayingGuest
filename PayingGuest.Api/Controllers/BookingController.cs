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
    public class BookingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetBookingByIdQuery { BookingId = id }, cancellationToken);

            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingCommand command, CancellationToken cancellationToken)
        {
            var booking = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = booking.BookingId }, booking);
        }
    }
}
