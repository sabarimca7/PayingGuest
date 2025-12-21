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
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly PayingGuestDbContext _context;

        public MaintenanceRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task<List<MaintenanceRequestDto>> GetByUserIdAsync(int userId)
        {
            return await _context.Maintenance
                .Where(m => m.ReportedBy == userId && m.IsActive)
                .OrderByDescending(m => m.CreatedDate)
                .Select(m => new MaintenanceRequestDto
                {
                    MaintenanceRequestId = m.MaintenanceId,
                    Issue = m.IssueType,
                    Description = m.Description,
                    DateRaised = m.CreatedDate,
                    Status = m.Status
                })
                .ToListAsync();
        }

    }
}
