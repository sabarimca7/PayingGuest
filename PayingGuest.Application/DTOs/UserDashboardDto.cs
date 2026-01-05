namespace PayingGuest.Application.DTOs
{
    public class UserDashboardDto
    {
        public string BookingStatus { get; set; } = string.Empty;
        // ✅ CHANGED TO DateTime
        public DateTime BookingSince { get; set; }


        public decimal MonthlyRent { get; set; }
        public DateTime NextDueDate { get; set; }

        public int PendingCount { get; set; }
        //public decimal PendingPaymentsAmount { get; set; }
        public decimal PendingTotalAmount { get; set; }
        public int StayDurationMonths { get; set; }

        // ✅ CHANGED TO DateTime
        public DateTime StaySince { get; set; }
    }
}
