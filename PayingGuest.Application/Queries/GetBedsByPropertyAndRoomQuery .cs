using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetBedsByPropertyAndRoomQuery : IRequest<ApiResponse<List<BedDto>>>
    {
        public int PropertyId { get; set; }
        public int RoomId { get; set; }

        public GetBedsByPropertyAndRoomQuery(int propertyId, int roomId)
        {
            PropertyId = propertyId;
            RoomId = roomId;
        }
    }
}
