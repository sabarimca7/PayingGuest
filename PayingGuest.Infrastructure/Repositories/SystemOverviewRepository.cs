using Microsoft.EntityFrameworkCore;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
  
using PayingGuest.Infrastructure.Data;

namespace PayingGuest.Infrastructure.Repositories
{
    public class SystemOverviewRepository : ISystemOverviewRepository
    {
        private readonly PayingGuestDbContext _context;

        public SystemOverviewRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task<SystemOverview> GetSystemOverviewAsync()
        {
            var totalProperties = await _context.Property.CountAsync();
            var activeProperties = await _context.Property.CountAsync(p => p.IsActive);

            var totalRooms = await _context.Room.CountAsync();

            var occupiedRooms = await _context.Booking
                .Where(b => b.Status == "Booked" || b.Status == "Active")
                .Select(b => b.BedId)
                .Distinct()
                .CountAsync();

            var availableRooms = totalRooms - occupiedRooms;

            var pendingBookings = await _context.Booking
                .CountAsync(b => b.Status == "Pending");

            var totalRevenue = await _context.Payment
                .Where(p => p.Status == "Paid")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var occupancyPercentage = totalRooms == 0
                ? 0
                : (decimal)occupiedRooms / totalRooms * 100;

            return new SystemOverview
            {
                TotalProperties = totalProperties,
                ActiveProperties = activeProperties,
                TotalRooms = totalRooms,
                OccupiedRooms = occupiedRooms,
                AvailableRooms = availableRooms,
                OccupancyPercentage = Math.Round(occupancyPercentage, 2),
                PendingBookings = pendingBookings,
                TotalRevenue = totalRevenue,
                PendingPayments = "Paid"
            };
        }
    }
}
