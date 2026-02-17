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
    public class GetSystemOverviewHandler: IRequestHandler<GetSystemOverviewQuery, SystemOverviewDto>
    {
        private readonly ISystemOverviewRepository _repository;

        public GetSystemOverviewHandler(ISystemOverviewRepository repository)
        {
            _repository = repository;
        }

        public async Task<SystemOverviewDto> Handle(GetSystemOverviewQuery request,CancellationToken cancellationToken)
        {
            var data = await _repository.GetSystemOverviewAsync();

            return new SystemOverviewDto
            {
                TotalProperties = data.TotalProperties,
                ActiveProperties = data.ActiveProperties,
                TotalRooms = data.TotalRooms,
                OccupiedRooms = data.OccupiedRooms,
                AvailableRooms = data.AvailableRooms,
                OccupancyPercentage = data.OccupancyPercentage,
                PendingBookings = data.PendingBookings,
                TotalRevenue = data.TotalRevenue,
                PendingPayments = data.PendingPayments
            };
        }
    }
}
