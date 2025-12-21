using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs
{
    public class UpcomingPaymentDto
    {
        public int PaymentId { get; set; }
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }   // Due Soon / Overdue
    }
}
