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
            //int propertyId,
            decimal? minPrice,
            decimal? maxPrice,
            int? capacity)
        {
            var query = _context.Room
                //.Where(r => r.PropertyId == propertyId)
                .AsQueryable();

            if (minPrice.HasValue)
                query = query.Where(r => r.RentPerBed >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(r => r.RentPerBed <= maxPrice.Value);

            if (capacity.HasValue)
                query = query.Where(r => r.TotalBeds >= capacity.Value);

            return await query.ToListAsync();
        }
    }
}
