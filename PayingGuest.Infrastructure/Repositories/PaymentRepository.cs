using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
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
                    on p.BookingId equals b.BookingId   // ✅ FK join
                join u in _context.User.AsNoTracking()
                    on b.UserId equals u.UserId
                orderby p.PaymentDate ascending
                select new PaymentDetailsDto
                {
                    BookingNumber = b.BookingNumber,   // ✅ FROM Booking table
                    TransactionId = p.TransactionId,
                    PhoneNumber = u.PhoneNumber,
                    Amount = p.Amount,
                    Status = p.Status
                }
            ).ToListAsync();
        }


        // ✅ REQUIRED BY INTERFACE
        public async Task AddAsync(Payment payment)
        {
            payment.TransactionId = GenerateTransactionId();
            payment.PaymentDate = DateTime.UtcNow;
            payment.Status = "Paid";

            await _context.Payment.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }
    }


}
