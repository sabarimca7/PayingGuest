using MediatR;
using PayingGuest.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Commands
{
    public class CreateBookingCommand : IRequest<BookingDto>
    {
        public int BookingId { get; set; }
        public int PropertyId { get; set; }
        public int UserId { get; set; }

        public int BedId { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly? CheckOutDate { get; set; }

        // public DateOnly PlannedCheckOutDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal SecurityDeposit { get; set; }
        public string BookingType { get; set; } = "Monthly";
       public string? Status { get; set; }
        public string? SpecialRequests { get; set; }
        public int DurationMonths { get; set; }
    }
}
