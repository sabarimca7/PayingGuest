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
                  .NotEmpty()
                  .WithMessage("CheckInDate is required.");


            RuleFor(x => x.CheckOutDate)
                           .Must((cmd, checkout) =>
                           {
                               if (checkout == null)
                                   return true; // allow null

                               return checkout >= cmd.CheckInDate;
                           })
                           .WithMessage("CheckOutDate must be after CheckInDate.");

            RuleFor(x => x.MonthlyRent).GreaterThan(0);
            RuleFor(x => x.SecurityDeposit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.BookingType)
                .NotEmpty()
                .MaximumLength(20);
        }
    }
}
