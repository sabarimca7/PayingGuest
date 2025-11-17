using Microsoft.EntityFrameworkCore;
using PayingGuest.Application.Queries;
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
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(PayingGuestDbContext context) : base(context)
        {

        }
        public async Task<IEnumerable<Booking>> GetBookingListAsync()
        {
            var query = _dbSet.AsQueryable();

            //if (searchBookingsQuery.PropertyId.HasValue)
            //    query = query.Where(x => x.PropertyId == searchBookingsQuery.PropertyId);

            //if (searchBookingsQuery.UserId.HasValue)
            //    query = query.Where(x => x.UserId == searchBookingsQuery.UserId);

            //if (searchBookingsQuery.BedId.HasValue)
            //    query = query.Where(x => x.BedId == searchBookingsQuery.BedId);

            //if (!string.IsNullOrWhiteSpace(searchBookingsQuery.Status))
            //    query = query.Where(x => x.Status == searchBookingsQuery.Status);

            //if (searchBookingsQuery.FromDate.HasValue)
            //    query = query.Where(x => x.CheckInDate >= searchBookingsQuery.FromDate);

            //if (searchBookingsQuery.ToDate.HasValue)
            //    query = query.Where(x => x.CheckInDate <= searchBookingsQuery.ToDate);

            return await query.ToListAsync();
        }
    }
}
