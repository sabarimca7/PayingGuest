using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class SystemOverview
    {
        public int TotalProperties { get; set; }
        public int ActiveProperties { get; set; }
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public decimal OccupancyPercentage { get; set; }
        public int AvailableRooms { get; set; }
        public int PendingBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public string PendingPayments { get; set; } = "Paid";
    }
}
