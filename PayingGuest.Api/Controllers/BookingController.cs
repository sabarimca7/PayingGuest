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

        // ----------------------------------------------------
        // GET: api/booking/{id}
        // ----------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetBookingByIdQuery { BookingId = id }, cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        

        // ----------------------------------------------------
        // POST: api/booking
        // ----------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingCommand command, CancellationToken cancellationToken)
        {
            var booking = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = booking.BookingId }, booking);
        }

        // ----------------------------------------------------
        // PUT: api/booking/{id}
        // ----------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<ActionResult<BookingDto>> Update(int id, [FromBody] UpdateBookingCommand command, CancellationToken cancellationToken)
        {
            if (id != command.BookingId)
                return BadRequest("Booking ID mismatch");

            var result = await _mediator.Send(command, cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // ----------------------------------------------------
        // DELETE (Soft Delete): api/booking/{id}
        // ----------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Cancel(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CancelBookingCommand { BookingId = id }, cancellationToken);

            if (!result)
                return NotFound();

            return NoContent();
        }

        // ----------------------------------------------------
        // GET: api/booking/search
        // Example:
        //   api/booking/search?propertyId=1&status=Active&page=1&pageSize=10
        // ----------------------------------------------------
        [HttpGet("search")]
        public async Task<ActionResult<List<BookingDto>>> Search(
            [FromQuery] SearchBookingsQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
       
    }
}
