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
    public class GetDashboardHandler
        : IRequestHandler<GetDashboardQuery, DashboardDto>
    {
        private readonly IDashboardRepository _repo;

        public GetDashboardHandler(IDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<DashboardDto> Handle(
            GetDashboardQuery request,
            CancellationToken cancellationToken)
        {
            return new DashboardDto
            {
                TotalProperties = await BuildMetric(
                    _repo.GetLastMonthPropertiesAsync,
                    _repo.GetTotalPropertiesAsync),

                TotalUsers = await BuildMetric(
                    _repo.GetLastMonthUsersAsync,
                    _repo.GetTotalUsersAsync),

                ActiveTenants = await BuildMetric(
                    _repo.GetLastMonthActiveTenantsAsync,
                    _repo.GetActiveTenantsAsync),

                MonthlyRevenue = await BuildMetric(
                    _repo.GetLastMonthRevenueAsync,
                    _repo.GetCurrentMonthRevenueAsync)
            };
        }

        private async Task<MetricDto> BuildMetric<T>(
            Func<Task<T>> lastMonth,
            Func<Task<T>> currentMonth) where T : struct
        {
            decimal last = Convert.ToDecimal(await lastMonth());
            decimal current = Convert.ToDecimal(await currentMonth());

            return new MetricDto
            {
                Value = current,
                GrowthPercentage = CalculateGrowth(last, current)
            };
        }

        private static decimal CalculateGrowth(decimal last, decimal current)
        {
            if (last == 0) return 100;
            return Math.Round(((current - last) / last) * 100, 2);
        }
    }
}
