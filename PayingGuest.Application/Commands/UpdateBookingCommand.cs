using MediatR;
using PayingGuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class UpdateBookingCommand : IRequest<BookingDto?>
    {
        public int BookingId { get; set; }
        public DateOnly? CheckOutDate { get; set; }
        public DateOnly PlannedCheckOutDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public string? SpecialRequests { get; set; }
    }
}
