using Microsoft.EntityFrameworkCore;
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
    public class BedRepository : IBedRepository
    {
        private readonly PayingGuestDbContext _context;

        public BedRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task<List<Bed>> GetBedsByPropertyAndRoomAsync(int propertyId, int roomId)
        {
            return await _context.Bed
              .Where(b => b.RoomId == roomId && b.Room.PropertyId == propertyId)
              .Include(b => b.Room)
              .ToListAsync();
        }
    }
}

