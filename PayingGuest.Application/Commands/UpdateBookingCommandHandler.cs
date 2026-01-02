using AutoMapper;
using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, BookingDto?>
    {
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public UpdateBookingCommandHandler(IUnitOfWork context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BookingDto?> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _context.Bookings .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);

            if (booking == null) return null;

            booking.MonthlyRent = request.MonthlyRent;
          //  booking.PlannedCheckOutDate = request.PlannedCheckOutDate;
            booking.SpecialRequests = request.SpecialRequests;
          //  booking.LastModifiedDate = DateTime.UtcNow;

            if (request.CheckOutDate.HasValue)
            {
                booking.CheckOutDate = request.CheckOutDate.Value;
               // booking.Status = "CheckedOut";
               // booking.IsActive = false;
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<BookingDto>(booking);
        }
    }
}
