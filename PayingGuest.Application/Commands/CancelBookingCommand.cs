using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class CancelBookingCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
    }
}
