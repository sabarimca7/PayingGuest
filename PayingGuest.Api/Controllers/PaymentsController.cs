using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayingGuest.Application.Queries;

namespace PayingGuest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetPaymentDetails()
        {
            var result = await _mediator.Send(new GetPaymentDetailsQuery());
            return Ok(result);
        }
    }
}
