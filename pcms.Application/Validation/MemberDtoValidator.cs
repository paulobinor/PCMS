using FluentValidation;
using pcms.Application.Dto;
using System;

namespace pcms.Application.Validation
{
    public class MemberDtoValidator : AbstractValidator<MemberDto>
    {
        public MemberDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("First Name is required")
                .MaximumLength(50).WithMessage("First Name cannot exceed 50 characters");

            //RuleFor(x => x.LastName)
            //    .NotEmpty().WithMessage("Last Name is required")
            //    .MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Invalid phone number format");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required")
                .Must(date => date <= DateTime.UtcNow.AddYears(-18))
                .WithMessage("Member must be at least 18 years old");

            //RuleFor(x => x.Employer)
            //    .NotEmpty().WithMessage("Employer is required");
        }
    }

}
