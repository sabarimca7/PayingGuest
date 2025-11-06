using FluentValidation;
using PayingGuest.Application.Commands;
using PayingGuest.Application.DTOs;
using System;
using System.Linq;

namespace PayingGuest.Application.Validators
{

    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be greater than 0");

            RuleFor(x => x.UpdateUserDto)
                .NotNull().WithMessage("Update data is required")
                .SetValidator(new UpdateUserValidator());
        }
    }
    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters and spaces");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Last name can only contain letters and spaces");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");

            RuleFor(x => x.AlternatePhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid alternate phone number format")
                .When(x => !string.IsNullOrEmpty(x.AlternatePhoneNumber));

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past")
                .GreaterThan(DateTime.Today.AddYears(-120)).WithMessage("Invalid date of birth")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.Gender)
                .Must(BeValidGender).WithMessage("Gender must be Male, Female, or Other")
                .When(x => !string.IsNullOrEmpty(x.Gender));

            RuleFor(x => x.BloodGroup)
                .Must(BeValidBloodGroup).WithMessage("Invalid blood group")
                .When(x => !string.IsNullOrEmpty(x.BloodGroup));

            RuleFor(x => x.EmergencyContactNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid emergency contact number")
                .When(x => !string.IsNullOrEmpty(x.EmergencyContactNumber));

            RuleFor(x => x.PermanentAddress)
                .MaximumLength(500).WithMessage("Permanent address must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.PermanentAddress));

            RuleFor(x => x.CurrentAddress)
                .MaximumLength(500).WithMessage("Current address must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.CurrentAddress));

            RuleFor(x => x.Occupation)
                .MaximumLength(100).WithMessage("Occupation must not exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Occupation));

            RuleFor(x => x.Company)
                .MaximumLength(200).WithMessage("Company name must not exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.Company));

            RuleFor(x => x.AadharNumber)
                .Matches(@"^\d{12}$").WithMessage("Aadhar number must be exactly 12 digits")
                .When(x => !string.IsNullOrEmpty(x.AadharNumber));

            RuleFor(x => x.PanNumber)
                .Matches(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$").WithMessage("Invalid PAN number format (e.g., ABCDE1234F)")
                .When(x => !string.IsNullOrEmpty(x.PanNumber));
        }

        private bool BeValidGender(string? gender)
        {
            if (string.IsNullOrEmpty(gender)) return true;
            var validGenders = new[] { "Male", "Female", "Other", "M", "F", "O" };
            return validGenders.Contains(gender, StringComparer.OrdinalIgnoreCase);
        }

        private bool BeValidBloodGroup(string? bloodGroup)
        {
            if (string.IsNullOrEmpty(bloodGroup)) return true;
            var validBloodGroups = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            return validBloodGroups.Contains(bloodGroup, StringComparer.OrdinalIgnoreCase);
        }
    }
}