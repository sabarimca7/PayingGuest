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
    public class GetUserDashboardHandler: IRequestHandler<GetUserDashboardQuery, UserDashboardDto>
    {
        private readonly IUserDashboardRepository _repository;

        public GetUserDashboardHandler(IUserDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDashboardDto> Handle(GetUserDashboardQuery request,CancellationToken cancellationToken)
        {
            return await _repository.GetUserDashboardAsync(request.UserId);
        }
    }

}
