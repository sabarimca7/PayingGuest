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
    public class DashboardRepository : IDashboardRepository
    {
        private readonly PayingGuestDbContext _context;

        public DashboardRepository(PayingGuestDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalPropertiesAsync()
            => await _context.Property.CountAsync();

        public async Task<int> GetLastMonthPropertiesAsync()
            => await _context.Property
                .Where(p => p.CreatedDate >= DateTime.Now.AddMonths(-1)
                         && p.CreatedDate < DateTime.Now)
                .CountAsync();

        //public async Task<int> GetTotalUsersAsync()
        //    => await _context.User.CountAsync();

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.User
                .Where(u => u.UserType != "Admin")
                .CountAsync();
        }

        //public async Task<int> GetLastMonthUsersAsync()
        //    => await _context.User
        //        .Where(u => u.CreatedDate >= DateTime.Now.AddMonths(-1)
        //                 && u.CreatedDate < DateTime.Now)
        //        .CountAsync();

        public async Task<int> GetLastMonthUsersAsync()
        {
            return await _context.User
                .Where(u => u.UserType != "Admin"
                         && u.CreatedDate >= DateTime.Now.AddMonths(-1)
                         && u.CreatedDate < DateTime.Now)
                .CountAsync();
        }


        //public async Task<int> GetActiveTenantsAsync()
        //    => await _context.User
        //        .Where(t => t.IsActive)
        //        .CountAsync();

        public async Task<int> GetActiveTenantsAsync()
        {
            return await _context.User
                .Where(u => u.UserType == "Tenant" && u.IsActive)
                .CountAsync();
        }


        //public async Task<int> GetLastMonthActiveTenantsAsync()
        //    => await _context.User
        //        .Where(t => t.IsActive &&
        //                    t.CreatedDate >= DateTime.Now.AddMonths(-1))
        //        .CountAsync();

        public async Task<int> GetLastMonthActiveTenantsAsync()
        {
            return await _context.User
                .Where(u => u.UserType == "Tenant"
                         && u.IsActive
                         && u.CreatedDate >= DateTime.Now.AddMonths(-1)
                         && u.CreatedDate < DateTime.Now)
                .CountAsync();
        }


        public async Task<decimal> GetCurrentMonthRevenueAsync()
            => await _context.Payment
                .Where(p => p.PaymentDate.Month == DateTime.Now.Month
                         && p.PaymentDate.Year == DateTime.Now.Year)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

        public async Task<decimal> GetLastMonthRevenueAsync()
            => await _context.Payment
                .Where(p => p.PaymentDate.Month == DateTime.Now.AddMonths(-1).Month
                         && p.PaymentDate.Year == DateTime.Now.Year)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;
    }
}
