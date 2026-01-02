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
    public class FilterRoomsQueryHandler: IRequestHandler<FilterQuery, List<RoomDto>>
    {
        private readonly IFilterRepository _roomFilterRepository;

        public FilterRoomsQueryHandler(IFilterRepository roomFilterRepository)
        {
            _roomFilterRepository = roomFilterRepository;
        }

        public async Task<List<RoomDto>> Handle(FilterQuery request,  CancellationToken cancellationToken)
        {
            var rooms = await _roomFilterRepository.FilterRoomsAsync(
                //request.Filter.PropertyId,
                request.Filter.MinPrice,
                request.Filter.MaxPrice,
                request.Filter.Capacity
            );

            return rooms.Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName,
                RoomType = r.RoomType,
                RentPerBed = r.RentPerBed,
                TotalBeds = r.TotalBeds,
                PropertyId = r.PropertyId,
                Image = r.RoomId == 2 ? "assets/img/single.jpg" : r.RoomId == 5 ? "assets/img/double.jpg" : "assets/img/triple.jpg",

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