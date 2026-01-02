using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayingGuest.Application.Queries;

namespace PayingGuest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            int userId = 8; // temporary

            var result = await _mediator.Send(new GetUserDashboardQuery
            {
                UserId = userId
            });

            return Ok(result);
        }

        [HttpGet("current-booking")]
        public async Task<IActionResult> GetCurrentBooking()
        {
            int userId = 8; // temporary / from JWT later

            var booking = await _mediator.Send(
                new GetCurrentBookingDetailsQuery { UserId = userId }
            );

            if (booking == null)
                return NotFound("No active booking found");

            return Ok(booking);
        }

        [HttpGet("{roomId}/amenities")] 
        public async Task<IActionResult> GetRoomAmenities(int roomId)
        {
            var result = await _mediator.Send(new GetRoomAmenitiesQuery(roomId));
            return Ok(result);
        }
        [HttpGet("upcoming/{userId}")]
        public async Task<IActionResult> GetUpcomingPayments(int userId)
        {
            var result = await _mediator.Send(
                new GetUpcomingPaymentsQuery(userId));

            return Ok(result);
        }
        [HttpGet("usermaintenance/{userId}")]
        public async Task<IActionResult> GetUserMaintenanceRequests(int userId)
        {
            var result = await _mediator.Send(
                new GetMaintenanceRequestsQuery(userId));

            return Ok(result);
        }

        [HttpGet("Bookings/recent/{userId}")]
        public async Task<IActionResult> GetRecentBookings(int userId)
        {
            var result = await _mediator.Send(new GetRecentBookingsQuery
            {
                UserId = userId,
                Take = 5
            });

            return Ok(result);
        }
    }
}
