using MediatR;
using PayingGuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetUserRecentBookingsQuery: IRequest<List<RecentBookingDto>>
    {
        public int UserId { get; set; }
        public int Take { get; set; } = 5;
    }
}
