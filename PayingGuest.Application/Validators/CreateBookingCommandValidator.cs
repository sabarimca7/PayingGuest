using FluentValidation;
using PayingGuest.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayingGuest.Application.Validators
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(x => x.PropertyId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.BedId).GreaterThan(0);

            RuleFor(x => x.CheckInDate)
                .LessThanOrEqualTo(x => x.PlannedCheckOutDate)
                .WithMessage("Check-in date must be before or equal to planned checkout date.");

            RuleFor(x => x.MonthlyRent).GreaterThan(0);
            RuleFor(x => x.SecurityDeposit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.BookingType)
                .NotEmpty()
                .MaximumLength(20);
        }
    }
}
