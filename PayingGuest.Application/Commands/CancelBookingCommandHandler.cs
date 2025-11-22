using MediatR;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
    {
        private readonly IUnitOfWork _context;

        public CancelBookingCommandHandler(IUnitOfWork context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(x => x.BookingId == request.BookingId);

            if (booking == null) return false;

          //  booking.IsActive = false;
          //  booking.Status = "Cancelled";
         //   booking.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
