using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayingGuest.Application.Queries;

namespace PayingGuest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _mediator.Send(new GetDashboardQuery());
            return Ok(result);
        }
        [HttpGet("recent-bookings")]
        public async Task<IActionResult> GetRecentBookings([FromQuery] int take = 5)
        {
            var result = await _mediator.Send(new GetAdminRecentBookingsQuery
            {
                Take = take
            });

            return Ok(result);
        }


        [HttpGet("recent-payments")]
        public async Task<IActionResult> GetRecentPayments([FromQuery] int take = 5)
        {
            var result = await _mediator.Send(new GetRecentPaymentsQuery
            {
                Take = take
            });

            return Ok(result);
        }



        [HttpGet("system-overview")]
        public async Task<IActionResult> GetSystemOverview()
        {
            return Ok(await _mediator.Send(new GetSystemOverviewQuery()));
        }
    }

}

