using Microsoft.EntityFrameworkCore;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Repositories
{
    public class FilterRepository : IFilterRepository
    {
        private readonly PayingGuestDbContext _context;

        public FilterRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

public async Task<List<Room>> FilterRoomsAsync(
    decimal? minPrice,
    decimal? maxPrice,
    int? capacity)
{
    // 🔒 Price validation (6 digits)
    if (minPrice.HasValue && minPrice > 999999)
        throw new ArgumentException("Min price cannot exceed 6 digits");

    if (maxPrice.HasValue && maxPrice > 999999)
        throw new ArgumentException("Max price cannot exceed 6 digits");

    // 🔒 Negative values
    if (minPrice < 0 || maxPrice < 0)
        throw new ArgumentException("Price cannot be negative");

    // 🔒 Capacity validation
    if (capacity.HasValue && capacity < 1)
        throw new ArgumentException("Capacity must be at least 1");

    // 🔒 Logical validation
    if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
        throw new ArgumentException("Min price cannot be greater than Max price");

    // 🔍 Filtering logic
    IQueryable<Room> query = _context.Room.AsQueryable();

    if (minPrice.HasValue)
        query = query.Where(r => r.RentPerBed >= minPrice);

    if (maxPrice.HasValue)
        query = query.Where(r => r.RentPerBed <= maxPrice);

    if (capacity.HasValue)
        query = query.Where(r => r.TotalBeds >= capacity);

    return await query.ToListAsync();
}

    }
}
