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
    public class GetMaintenanceRequestsHandler : IRequestHandler<GetMaintenanceRequestsQuery, List<MaintenanceRequestDto>>
    {
        private readonly IMaintenanceRepository _repository;

        public GetMaintenanceRequestsHandler(IMaintenanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MaintenanceRequestDto>> Handle(GetMaintenanceRequestsQuery request,CancellationToken cancellationToken)
        {
            return await _repository.GetByUserIdAsync(request.UserId);
        }
    }
}
