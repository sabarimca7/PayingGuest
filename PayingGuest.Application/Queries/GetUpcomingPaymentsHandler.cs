using MediatR;
using PayingGuest.Application.DTOs;
using PayingGuest.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public class GetUpcomingPaymentsHandler : IRequestHandler<GetUpcomingPaymentsQuery, List<UpcomingPaymentDto>>
    {
        private readonly IUpcomingPaymentRepository _repository;

        public GetUpcomingPaymentsHandler(IUpcomingPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UpcomingPaymentDto>> Handle(GetUpcomingPaymentsQuery request,CancellationToken cancellationToken)
        {
            return await _repository.GetUpcomingPaymentsAsync(request.UserId);
        }
    }
}