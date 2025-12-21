using MediatR;
using PayingGuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetCurrentBookingDetailsQuery
    : IRequest<CurrentBookingDetailsDto>
    {
        public int UserId { get; set; }
    }
}
