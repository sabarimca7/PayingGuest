using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalPropertiesAsync();
        Task<int> GetLastMonthPropertiesAsync();

        Task<int> GetTotalUsersAsync();
        Task<int> GetLastMonthUsersAsync();

        Task<int> GetActiveTenantsAsync();
        Task<int> GetLastMonthActiveTenantsAsync();

        Task<decimal> GetCurrentMonthRevenueAsync();
        Task<decimal> GetLastMonthRevenueAsync();
        
    }
}
