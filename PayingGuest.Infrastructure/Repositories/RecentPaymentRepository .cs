using Microsoft.EntityFrameworkCore;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Data;

namespace PayingGuest.Infrastructure.Repositories
{
    public class RecentPaymentRepository : IRecentPaymentRepository
    {
        private readonly PayingGuestDbContext _context;

        public RecentPaymentRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        // ✅ FIXED METHOD NAME (Async, not AsynC)
        public async Task<List<RecentPaymentDto>> GetRecentPaymentsAsync(int take)
        {
            var query =
                from payment in _context.Payment
                join booking in _context.Booking
                    on payment.BookingId equals booking.BookingId
                join user in _context.User
                    on booking.UserId equals user.UserId
                join property in _context.Property
                    on booking.PropertyId equals property.PropertyId
                orderby payment.PaymentDate descending
                select new RecentPaymentDto
                {
                    PaymentNumber = payment.PaymentNumber,
                    TenantName = user.FirstName,
                    PropertyName = property.PropertyName,
                    PaymentType = payment.PaymentType,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate
                };

            return await query.Take(take).ToListAsync();
        }

    }
}
