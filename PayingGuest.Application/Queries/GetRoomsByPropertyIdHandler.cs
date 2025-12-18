using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{

    public class GetRoomsByPropertyIdHandler
        : IRequestHandler<GetRoomsByPropertyIdQuery, List<RoomDto>>
    {
        private readonly IRoomRepository _roomRepository;

        public GetRoomsByPropertyIdHandler(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<List<RoomDto>> Handle(
            GetRoomsByPropertyIdQuery request,
            CancellationToken cancellationToken)
        {
            var rooms = await _roomRepository
                .GetRoomsByPropertyIdAsync(request.PropertyId);



            return rooms.Select(r => new RoomDto
            {
                PropertyId = r.PropertyId,
                RoomId = r.RoomId,
                RoomType = r.RoomType,
                RoomName = r.RoomName,
                TotalBeds = r.TotalBeds,
                RentPerBed = r.RentPerBed,
                Image = r.RoomId == 2 ? "assets/img/single.jpg" : r.RoomId == 5? "assets/img/double.jpg": "assets/img/triple.jpg",

                // ⭐ Features generated here (BEST PRACTICE)
                Features = new List<string>
                {
                    $"Max {r.TotalBeds}",   // ONE Max per room
                    "AC",
                    "Bathroom",
                    "Study Table",
                    "Wardrobe"

                }


            }).ToList();
        }
    }
}