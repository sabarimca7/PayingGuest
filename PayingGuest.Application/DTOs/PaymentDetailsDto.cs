using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.DTOs
{
    public class PaymentDetailsDto
    {
        public string TransactionId { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}
