using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string BookingNumber { get; set; } = default!;
        public int PropertyId { get; set; }
        public int UserId { get; set; }
        public int BedId { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }

        //public DateOnly PlannedCheckOutDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal SecurityDeposit { get; set; }

        public string Status { get; set; } = "Booked";
        public string BookingType { get; set; } = default!; // Monthly/Daily etc.
        public string? SpecialRequests { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
       public string? LastModifiedBy { get; set; }
    }
}
