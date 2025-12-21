using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs
{
    public class DashboardDto
    {
        public MetricDto TotalProperties { get; set; } = new();
        public MetricDto TotalUsers { get; set; } = new();
        public MetricDto ActiveTenants { get; set; } = new();
        public MetricDto MonthlyRevenue { get; set; } = new();
    }

    public class MetricDto
    {
        public decimal Value { get; set; }
        public decimal GrowthPercentage { get; set; }
    }
}
