using Microsoft.EntityFrameworkCore;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Infrastructure.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly PayingGuestDbContext _context;

        public ProfileRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileSummaryDto?> GetUserProfileSummaryAsync(int userId)
        {
            var user = await _context.User
                .Where(u => u.UserId == userId)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.EmailAddress,
                    u.PhoneNumber,
                    u.IsActive
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            int totalBookings = await _context.Booking
                .CountAsync(b => b.UserId == userId);

            return new ProfileSummaryDto
            {
                Name = $"{user.FirstName} {user.LastName}",
                Email = user.EmailAddress,
                Phone = user.PhoneNumber,
                TotalBookings = totalBookings,
                Status = user.IsActive ? "Active" : "Inactive"
            };
        }
    }
    
}