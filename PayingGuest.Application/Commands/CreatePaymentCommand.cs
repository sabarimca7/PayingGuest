using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class CreatePaymentCommand : IRequest<int>
    {
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
       
    }
}
