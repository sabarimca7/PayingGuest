using Microsoft.EntityFrameworkCore;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Data;
using System;

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
            .Where(b => b.UserId == userId && b.Status == "Booked")
            .Select(b => new
            {
                b.Status,
                b.CreatedDate,
                b.DurationMonths,
                b.CheckInDate,
                RentPerBed = b.Bed.Room.RentPerBed,
                BookingId = b.BookingId
            })
            .FirstOrDefaultAsync();

        if (booking == null)
            return new UserDashboardDto();


        // 2️⃣ Convert CheckInDate (DateOnly → DateTime)
        DateTime checkInDate =
            booking.CheckInDate.ToDateTime(TimeOnly.MinValue);

        // 3️⃣ Format "Since" text (THIS IS THE KEY LINE)
        string bookingSince =
            $"Since {checkInDate:MMM d, yyyy}";

        // 3️⃣ Find last PAID monthly rent (if any)
        var lastPaidRentDate = await _context.Payment
            .Where(p =>
                p.BookingId == booking.BookingId &&
                p.PaymentType == "MonthlyRent" &&
                p.Status == "Paid")
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => p.PaymentDate)
            .FirstOrDefaultAsync();

        // 4️⃣ Calculate next due date
        DateTime nextDueDate;

        if (lastPaidRentDate != default)
        {
            // If rent already paid → next month after last payment
            nextDueDate = lastPaidRentDate.AddMonths(1);
        }
        else
        {
            // First rent due → one month after check-in
            nextDueDate = checkInDate.AddMonths(1);
        }

        // 1️⃣ Get all pending payments for the user
        var pendingPayments = await _context.Payment
            .Where(p =>
                p.Booking.UserId == userId &&
                p.Status != "Paid" &&          // 🔴 pending condition
                p.IsActive
            )
            .Select(p => p.Amount)
            .ToListAsync();

        // 2️⃣ Calculate count & total
        int pendingCount = pendingPayments.Count;
        decimal pendingTotalAmount = pendingPayments.Sum();

        // 2️⃣ Calculate months stayed
        int monthsStayed =
            ((DateTime.Now.Year - booking.CheckInDate.Year) * 12) +
            (DateTime.Now.Month - booking.CheckInDate.Month);

        if (monthsStayed < 0)
            monthsStayed = 0;

        // 3️⃣ Calculate "Since" text
        string staySince = $"{booking.CheckInDate:MMM yyyy} ({monthsStayed} months)";


        return new UserDashboardDto
        {
            BookingStatus = booking.Status,
            BookingSince = bookingSince,
            MonthlyRent = booking.RentPerBed,
            PendingCount = pendingCount,
            PendingTotalAmount = pendingTotalAmount,
            StayDurationMonths = booking.DurationMonths,
            StaySince = staySince,
            NextDueDate = nextDueDate
        };
    }
    public async Task<CurrentBookingDetailsDto?> GetCurrentBookingDetailsAsync(int userId)
    {
        var result = await
        (
            from b in _context.Booking

            join pr in _context.Property
                on b.PropertyId equals pr.PropertyId

            join bd in _context.Bed
                on b.BedId equals bd.BedId

            join r in _context.Room
                on bd.RoomId equals r.RoomId

            join f in _context.Floor
                on r.FloorId equals f.FloorId

            join pay in _context.Payment
                on b.BookingId equals pay.BookingId
                into payments
            from p in payments.DefaultIfEmpty()

            where b.UserId == userId
                  && b.Status == "Booked"

            group new { b, pr, r, f, p } by new
            {
                b.BookingId,
                pr.PropertyName,
                pr.Address,
                pr.ContactNumber,

                r.RoomNumber,
                r.RoomType,

                f.FloorNumber,

                r.RentPerBed,
                b.CheckInDate
            }
            into g

            select new CurrentBookingDetailsDto
            {
                PropertyName = g.Key.PropertyName,
                Address = g.Key.Address,
                OwnerContact = g.Key.ContactNumber,

                RoomNo = g.Key.RoomNumber,
                RoomType = g.Key.RoomType,
                Floor = g.Key.FloorNumber,

                // ✅ ALREADY DateOnly — NO CONVERSION
                MoveInDate = g.Key.CheckInDate.ToDateTime(TimeOnly.MinValue),


                ContractMonths = 12, // business rule

                MonthlyRent = g.Key.RentPerBed,

                // ✅ FIXED: Deposit only
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




