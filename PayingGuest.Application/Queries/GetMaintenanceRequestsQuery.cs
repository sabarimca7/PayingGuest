using MediatR;
using PayingGuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Queries
{
    public record GetMaintenanceRequestsQuery(int UserId)
       : IRequest<List<MaintenanceRequestDto>>;
}
