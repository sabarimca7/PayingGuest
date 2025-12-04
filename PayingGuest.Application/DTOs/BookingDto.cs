using System;

namespace PayingGuest.Application.DTOs
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public string BookingNumber { get; set; } = default!;
        public int PropertyId { get; set; }
        public int UserId { get; set; }
        public int BedId { get; set; }

        public DateOnly CheckInDate { get; set; }
        public DateOnly? CheckOutDate { get; set; }

        public decimal MonthlyRent { get; set; }
        public decimal SecurityDeposit { get; set; }

        public string Status { get; set; } = "Booked";

        public string? SpecialRequests { get; set; }
        public int DurationMonths { get; set; }

        public string PaymentMethod { get; set; } = default!;
        public string PaymentNotes { get; set; } = default!;
    }
}
