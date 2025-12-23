namespace PayingGuest.Application.DTOs
{
    public class UserDashboardDto
    {
        public string BookingStatus { get; set; } = string.Empty;
        public string BookingSince { get; set; } = string.Empty;

        public decimal MonthlyRent { get; set; }
        public DateTime NextDueDate { get; set; }

        public int PendingCount { get; set; }
        //public decimal PendingPaymentsAmount { get; set; }
        public decimal PendingTotalAmount { get; set; }
        public int StayDurationMonths { get; set; }
        public string StaySince { get; set; } = string.Empty;
    }
}
