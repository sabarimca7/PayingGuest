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
    public class GetUserRecentBookingsHandler : IRequestHandler<GetUserRecentBookingsQuery, List<RecentBookingDto>>
    {
        private readonly IRecentBookingRepository _repository;

        public GetUserRecentBookingsHandler(IRecentBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RecentBookingDto>> Handle( GetUserRecentBookingsQuery request,CancellationToken cancellationToken)
        {
            var bookings = await _repository.GetRecentBookingsByUserAsync(
                request.UserId,
                request.Take
            );

            return bookings.Select(b => new RecentBookingDto
            {
                BookingNumber = b.BookingNumber,
                TenantName = b.User.FirstName,
                PropertyName = b.Property.PropertyName,
                RoomNumber = b.Bed?.Room?.RoomNumber ?? "N/A",
                MoveInDate = b.CheckInDate.ToDateTime(TimeOnly.MinValue),
                Rent = b.MonthlyRent
            }).ToList();
        }
    }
}
