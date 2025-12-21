namespace PayingGuest.Application.DTOs
{
    public class RecentPaymentDto
    {
        public string PaymentNumber { get; set; }
        public string TenantName { get; set; }
        public string PropertyName { get; set; } = "Karthick PG";
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
