using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetRoomAmenitiesHandler
       : IRequestHandler<GetRoomAmenitiesQuery, RoomAmenitiesDto>
    {
        private readonly IRoomAmenitiesRepository _repository;

        public GetRoomAmenitiesHandler(IRoomAmenitiesRepository repository)
        {
            _repository = repository;
        }

        public async Task<RoomAmenitiesDto> Handle(
            GetRoomAmenitiesQuery request,
            CancellationToken cancellationToken)
        {
            var amenities = await _repository.GetAmenitiesByRoomIdAsync(request.RoomId);

            return new RoomAmenitiesDto
            {
                RoomId = request.RoomId,
                Amenities = amenities ?? string.Empty
            };
        }
    }
}
