using Microsoft.EntityFrameworkCore;
using PayingGuest.Application.Interfaces;
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

    public class RoomRepository : IRoomRepository
    {
        private readonly PayingGuestDbContext _context;

        public RoomRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetRoomsByPropertyIdAsync(int propertyId)
        {
            return await _context.Room
                .Where(r => r.PropertyId == propertyId && r.IsActive)
                .ToListAsync();
        }
    }
}