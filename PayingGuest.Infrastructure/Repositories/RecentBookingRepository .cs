using Microsoft.EntityFrameworkCore;
using PayingGuest.Application.DTOs;
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
    public class RecentBookingRepository : IRecentBookingRepository
    {
        private readonly PayingGuestDbContext _context;

        public RecentBookingRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetRecentBookingsAsync(int take)
        {
            return await (
                from b in _context.Booking
                join u in _context.User on b.UserId equals u.UserId
                join p in _context.Property on b.PropertyId equals p.PropertyId
                join bed in _context.Bed on b.BedId equals bed.BedId
                join r in _context.Room on bed.RoomId equals r.RoomId
                where b.IsActive
                orderby b.CreatedDate descending
                select new Booking
                {
                    BookingId = b.BookingId,
                    BookingNumber = b.BookingNumber,
                    PropertyId = b.PropertyId,
                    UserId = b.UserId,
                    BedId = b.BedId,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    MonthlyRent = b.MonthlyRent,
                    SecurityDeposit = b.SecurityDeposit,
                    Status = b.Status,
                    BookingType = b.BookingType,
                    SpecialRequests = b.SpecialRequests,
                    IsActive = b.IsActive,
                    CreatedDate = b.CreatedDate,
                    DurationMonths = b.DurationMonths,

                    // ✅ Manually populate navigation properties
                    User = u,
                    Property = p,
                    Bed = new Bed
                    {
                        BedId = bed.BedId,
                        RoomId = bed.RoomId,
                        Room = r
                    }
                }
            )
            .AsNoTracking()
            .Take(take)
            .ToListAsync();
        }
        public async Task<List<Booking>> GetRecentBookingsByUserAsync(int userId, int take)
        {
            return await (
                from b in _context.Booking
                join u in _context.User on b.UserId equals u.UserId
                join p in _context.Property on b.PropertyId equals p.PropertyId
                join bed in _context.Bed on b.BedId equals bed.BedId
                join r in _context.Room on bed.RoomId equals r.RoomId
                where b.IsActive && b.UserId == userId     // ✅ USER FILTER
                orderby b.CreatedDate descending
                select new Booking
                {
                    BookingId = b.BookingId,
                    BookingNumber = b.BookingNumber,
                    PropertyId = b.PropertyId,
                    UserId = b.UserId,
                    BedId = b.BedId,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    MonthlyRent = b.MonthlyRent,
                    SecurityDeposit = b.SecurityDeposit,
                    Status = b.Status,
                    BookingType = b.BookingType,
                    SpecialRequests = b.SpecialRequests,
                    IsActive = b.IsActive,
                    CreatedDate = b.CreatedDate,
                    DurationMonths = b.DurationMonths,

                    User = u,
                    Property = p,
                    Bed = new Bed
                    {
                        BedId = bed.BedId,
                        RoomId = bed.RoomId,
                        Room = r
                    }
                }
            )
            .AsNoTracking()
            .Take(take)
            .ToListAsync();
        }

    }
}