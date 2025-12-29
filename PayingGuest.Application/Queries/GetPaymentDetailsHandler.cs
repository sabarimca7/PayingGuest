using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetPaymentDetailsHandler
     : IRequestHandler<GetPaymentDetailsQuery, List<PaymentDetailsDto>>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentDetailsHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }


        public async Task<List<PaymentDetailsDto>> Handle(
              GetPaymentDetailsQuery request,
              CancellationToken cancellationToken)
        {
            return await _paymentRepository.GetPaymentDetailsAsync()
                   ?? new List<PaymentDetailsDto>();
        }
    }
}