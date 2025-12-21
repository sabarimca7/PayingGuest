using PayingGuest.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PayingGuest.Application.Interfaces
{
    public interface IRecentPaymentRepository
    {
        Task<List<RecentPaymentDto>> GetRecentPaymentsAsync(int take);
    }
}
