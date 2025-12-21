using PayingGuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Interfaces
{

    public interface IUpcomingPaymentRepository
    {
        Task<List<UpcomingPaymentDto>> GetUpcomingPaymentsAsync(int userId);
    }
}
