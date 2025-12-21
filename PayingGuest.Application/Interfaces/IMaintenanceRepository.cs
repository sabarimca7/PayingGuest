using PayingGuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Interfaces
{
    public interface IMaintenanceRepository
    {
        Task<List<MaintenanceRequestDto>> GetByUserIdAsync(int userId);
    }
}
