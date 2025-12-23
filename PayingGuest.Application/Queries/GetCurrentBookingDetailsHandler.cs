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

    public class GetCurrentBookingDetailsHandler
        : IRequestHandler<GetCurrentBookingDetailsQuery, CurrentBookingDetailsDto?>
    {
        private readonly IUserDashboardRepository _repository;

        public GetCurrentBookingDetailsHandler(IUserDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<CurrentBookingDetailsDto?> Handle(
            GetCurrentBookingDetailsQuery request,
            CancellationToken cancellationToken)
        {

            return await _repository.GetCurrentBookingDetailsAsync(request.UserId);
        }
    }
}