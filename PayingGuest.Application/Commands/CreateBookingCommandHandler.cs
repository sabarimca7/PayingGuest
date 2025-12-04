using AutoMapper;
using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateBookingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            // TODO: check bed availability (Booking with same BedId and active not checked out, etc.)

            var booking = new Booking
            {
                BookingNumber = await GenerateBookingNumberAsync(cancellationToken),
                PropertyId = request.PropertyId,
                UserId = request.UserId,
                BedId = request.BedId,
                CheckInDate = request.CheckInDate,
                CheckOutDate=request.CheckOutDate,
                MonthlyRent = request.MonthlyRent,
                SecurityDeposit = request.SecurityDeposit,
                BookingType = request.BookingType,
                Status = "Booked", // ✅ Correct position and syntax
                SpecialRequests = request.SpecialRequests,
                DurationMonths=request.DurationMonths,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "system"
            };
            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<BookingDto>(booking);
        }

        private async Task<string> GenerateBookingNumberAsync(CancellationToken cancellationToken)
        {
            // Simple example: "BKG-2025-0001"
            var count = await _unitOfWork.Bookings.CountAsync() + 1;
            var year = DateTime.UtcNow.Year;
            return $"BKG-{year}-{count:D4}";
        }
    }
}
