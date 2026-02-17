using MediatR;
using PayingGuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetAdminRecentBookingsQuery: IRequest<List<RecentBookingDto>>
    {
        public int Take { get; set; } = 5;
    }
}
