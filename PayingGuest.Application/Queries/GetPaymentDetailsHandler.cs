using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Application.Interfaces;
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
        private readonly IPaymentRepository _repo;

        public GetPaymentDetailsHandler(IPaymentRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<PaymentDetailsDto>> Handle(
            GetPaymentDetailsQuery request,
            CancellationToken cancellationToken)
        {
            return await _repo.GetPaymentDetailsAsync();
        }
    }
}