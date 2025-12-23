using Microsoft.EntityFrameworkCore;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PayingGuestDbContext _context;

        public PaymentRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentDetailsDto>> GetPaymentDetailsAsync()
        {
            return await (
                from p in _context.Payment.AsNoTracking()
                join b in _context.Booking.AsNoTracking()
                    on p.BookingId equals b.BookingId
                join u in _context.User.AsNoTracking()
                    on b.UserId equals u.UserId
                orderby p.PaymentDate descending
                select new PaymentDetailsDto
                {
                    TransactionId = p.TransactionId,
                    PhoneNumber = u.PhoneNumber,
                    Amount = p.Amount,
                    Status = p.Status
                }
            ).ToListAsync();
        }
    }
}
