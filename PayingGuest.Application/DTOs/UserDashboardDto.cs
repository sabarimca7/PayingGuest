namespace PayingGuest.Application.DTOs
{
    public class UserDashboardDto
    {
        public string BookingStatus { get; set; } = string.Empty;

        public decimal MonthlyRent { get; set; }

        public int PendingPaymentsCount { get; set; }
        public decimal PendingPaymentsAmount { get; set; }

        public int StayDurationMonths { get; set; }
    }
}
