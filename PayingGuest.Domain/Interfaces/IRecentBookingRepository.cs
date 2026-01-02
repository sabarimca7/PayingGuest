using PayingGuest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Interfaces
{
    public interface IRecentBookingRepository
    {
        Task<List<Booking>> GetRecentBookingsAsync(int take);
        Task<List<Booking>> GetRecentBookingsByUserAsync(int userId, int take); // User
    }
}
