using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Domain.Entities
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string BookingNumber { get; set; } = default!;

        // 🔑 Foreign Keys
        public int PropertyId { get; set; }
        public int UserId { get; set; }
        public int BedId { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly? CheckOutDate { get; set; }

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
        public int DurationMonths { get; set; }




        // ✅ Navigation properties
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public Property Property { get; set; }

        [ForeignKey(nameof(BedId))]
        public Bed Bed { get; set; }
        // ✅ Navigation property (ONE booking → MANY payments)
        public ICollection<Payment> Payment { get; set; } = new List<Payment>();
        // Navigation
       

    }

}
