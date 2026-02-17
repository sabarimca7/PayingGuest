using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs
{
    public class PaymentDto
    {
        public int BookingId { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = default!;

        // Optional
        public string? PaymentNotes { get; set; }
    }
}
