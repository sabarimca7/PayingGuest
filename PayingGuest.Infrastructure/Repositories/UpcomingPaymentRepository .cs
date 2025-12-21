using Microsoft.EntityFrameworkCore;
using PayingGuest.Application.DTOs;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Repositories
{
    public class UpcomingPaymentRepository : IUpcomingPaymentRepository
    {
        private readonly PayingGuestDbContext _context;

        public UpcomingPaymentRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task<List<UpcomingPaymentDto>> GetUpcomingPaymentsAsync(int userId)
        {
            var today = DateTime.Today;

            return await _context.Payment
                .Include(p => p.Booking)
                .Where(p =>
                    p.Booking.UserId == userId &&
                    p.Status != "Paid" &&
                    p.DueDate != null)
                .OrderBy(p => p.DueDate)
                .Select(p => new UpcomingPaymentDto
                {
                    PaymentId = p.PaymentId,
                    PaymentType = p.PaymentType,
                    Amount = p.Amount,
                    DueDate = p.DueDate!.Value,
                    Status = p.DueDate.Value <= today.AddDays(3)
                        ? "Due Soon"
                        : "Upcoming"
                })
                .ToListAsync();
        }
    }
}
