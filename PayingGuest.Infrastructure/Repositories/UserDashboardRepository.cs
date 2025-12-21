using Microsoft.EntityFrameworkCore;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Data;

public class UserDashboardRepository : IUserDashboardRepository
{
    private readonly PayingGuestDbContext _context;

    public UserDashboardRepository(PayingGuestDbContext context)
    {
        _context = context;
    }

    public async Task<UserDashboardDto> GetUserDashboardAsync(int userId)
    {
        // 1️⃣ Active booking (no Room, no MoveInDate)
        var booking = await _context.Booking
            .Where(b => b.UserId == userId && b.Status == "Active")
            .Select(b => new
            {
                b.CreatedDate
            })
            .FirstOrDefaultAsync();

        if (booking == null)
            return new UserDashboardDto();

        // 2️⃣ Pending payments
        var pendingPayments = await _context.Payment
            .Where(p => p.UserId == userId && p.Status == "Pending")
            .ToListAsync();

        // 3️⃣ Monthly rent (from last payment or fixed logic)
        var monthlyRent = await _context.Payment
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => p.Amount)
            .FirstOrDefaultAsync();

        // 4️⃣ Stay duration
        int stayMonths =
            ((DateTime.Now.Year - booking.CreatedDate.Year) * 12) +
            (DateTime.Now.Month - booking.CreatedDate.Month);

        return new UserDashboardDto
        {
            BookingStatus = "Active",
            MonthlyRent = monthlyRent,
            PendingPaymentsCount = pendingPayments.Count,
            PendingPaymentsAmount = pendingPayments.Sum(p => p.Amount),
            StayDurationMonths = stayMonths
        };
    }
    public async Task<CurrentBookingDetailsDto?> GetCurrentBookingDetailsAsync(int userId)
    {
        var result =
            await (
                from b in _context.Booking
                join pr in _context.Property
                    on b.PropertyId equals pr.PropertyId

                join pay in _context.Payment
                    on b.BookingId equals pay.BookingId
                    into payments   // LEFT JOIN
                from p in payments.DefaultIfEmpty()

                where b.UserId == userId
                      && b.Status == "Active"

                group new { b, pr, p } by new
                {
                    b.BookingId,
                    pr.PropertyName,
                    pr.Address,
                    b.CreatedDate
                }
                into g

                select new CurrentBookingDetailsDto
                {
                    PropertyName = g.Key.PropertyName,
                    Address = g.Key.Address,

                    MoveInDate = g.Key.CreatedDate,

                    // You said this field doesn't exist
                    ContractMonths = 12,

                    MonthlyRent = g
                        .Where(x => x.p != null && x.p.PaymentType == "Monthly")
                        .OrderByDescending(x => x.p.PaymentDate)
                        .Select(x => x.p.Amount)
                        .FirstOrDefault(),

                    SecurityDeposit = g
                        .Where(x => x.p != null && x.p.PaymentType == "Deposit")
                        .Select(x => x.p.Amount)
                        .FirstOrDefault(),

                    IsDepositPaid = g
                        .Any(x => x.p != null && x.p.PaymentType == "Deposit")
                }
            ).FirstOrDefaultAsync();

        return result;
    }
}
