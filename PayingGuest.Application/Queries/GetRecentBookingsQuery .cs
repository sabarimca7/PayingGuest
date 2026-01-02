using MediatR;
using PayingGuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetRecentBookingsQuery : IRequest<List<RecentBookingDto>>
    {
        public int Take { get; set; } = 5; // default last 5 bookings
        public int UserId { get; set; }
    }
}
