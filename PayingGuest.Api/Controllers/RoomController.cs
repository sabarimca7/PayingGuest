using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Queries;

namespace PayingGuest.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoomController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // GET: api/rooms/by-property/
        [HttpGet("by-property/{propertyId}")]
        public async Task<IActionResult> GetRoomsByPropertyId(int propertyId)
        {
            var rooms = await _mediator.Send(new GetRoomsByPropertyIdQuery(propertyId));

            if (rooms == null || rooms.Count == 0)
                return NotFound("No rooms found for this property");

            return Ok(rooms);
        }
        [HttpGet("{propertyId}/rooms/{roomId}/beds")]
        public async Task<IActionResult> GetBeds(int propertyId, int roomId)
        {
            var response = await _mediator.Send(new GetBedsByPropertyAndRoomQuery(propertyId, roomId));

            if (!response.Success)
                return NotFound(response);  // returns { success:false, errors:[...] }

            return Ok(response); // returns { success:true, data:[...] }
        }
        [HttpPost("filter")]
        public async Task<IActionResult> FilterRooms([FromBody] RoomFilterDto filter)
        {
            var result = await _mediator.Send(new FilterQuery(filter));
            return Ok(result);
        }
    }


}



