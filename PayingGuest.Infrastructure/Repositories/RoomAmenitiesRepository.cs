using Microsoft.EntityFrameworkCore;
using PayingGuest.Domain.Interfaces;
using PayingGuest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Repositories
{
    public class RoomAmenitiesRepository : IRoomAmenitiesRepository
    {
        private readonly PayingGuestDbContext _context;

        public RoomAmenitiesRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetAmenitiesByRoomIdAsync(int roomId)
        {
            return await _context.Room
                .Where(r => r.RoomId == roomId)
                .Select(r => r.Amenities)
                .FirstOrDefaultAsync();
        }
    }
}
