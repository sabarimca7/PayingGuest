using PayingGuest.Application.DTOs;
using PayingGuest.Domain.Entities;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Interfaces
{
  
        public interface IPaymentRepository
        {
       Task<List<PaymentDetailsDto>> GetPaymentDetailsAsync();

        Task AddAsync(Payment payment);
    }
    
}
