using FluentValidation;
using PayingGuest.Application.Commands;
using PayingGuest.Application.DTOs;

namespace PayingGuest.Application.Validators
{

    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            // Validate the RegisterUserDto property within the command
            RuleFor(x => x.RegisterUserDto)
                .NotNull().WithMessage("User registration data is required")
                .SetValidator(new RegisterUserDtoValidator()); // Reuse your DTO validator
        }
    }
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(x => x.PropertyId)
                .GreaterThan(0).WithMessage("Property ID must be greater than 0");

            RuleFor(x => x.UserType)
                .NotEmpty().WithMessage("User type is required")
                .Must(BeValidUserType).WithMessage("Invalid user type");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Gender)
                .Must(BeValidGender).WithMessage("Gender must be Male, Female, or Other")
                .When(x => !string.IsNullOrEmpty(x.Gender));

            RuleFor(x => x.AadharNumber)
                .Matches(@"^\d{12}$").WithMessage("Aadhar number must be 12 digits")
                .When(x => !string.IsNullOrEmpty(x.AadharNumber));

            RuleFor(x => x.PanNumber)
                .Matches(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$").WithMessage("Invalid PAN number format")
                .When(x => !string.IsNullOrEmpty(x.PanNumber));
        }

        private bool BeValidUserType(string userType)
        {
            var validTypes = new[] { "Admin", "Manager", "Guest", "Staff" };
            return validTypes.Contains(userType);
        }

        private bool BeValidGender(string? gender)
        {
            if (string.IsNullOrEmpty(gender)) return true;
            var validGenders = new[] { "Male", "Female", "Other" };
            return validGenders.Contains(gender);
        }
    }
}