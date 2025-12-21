using PayingGuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Interfaces
{
    public interface IUserDashboardRepository
    {
        Task<UserDashboardDto> GetUserDashboardAsync(int userId);
        Task<CurrentBookingDetailsDto?> GetCurrentBookingDetailsAsync(int userId);
    }
}
