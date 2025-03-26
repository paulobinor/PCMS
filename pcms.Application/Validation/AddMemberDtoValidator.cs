using FluentValidation;
using pcms.Application.Dto;

namespace pcms.Application.Validation
{
    public class AddMemberDtoValidator : AbstractValidator<AddMemberDto>
    {
        public AddMemberDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_.,]+$")
            .WithMessage("Only letters, numbers, spaces, and - _ . , are allowed.");

            RuleFor(x => x.RSAPin)
                .NotEmpty().WithMessage("RSA Pin is required")
                .MaximumLength(15).WithMessage("RSA Pin cannot exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_.,]+$")
            .WithMessage("Only letters, numbers, spaces, and - _ . , are allowed.");

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

            RuleFor(x => x.Employer)
                .NotEmpty().WithMessage("Employer is required")
            .Matches(@"^[a-zA-Z0-9\s\-_.,]+$")
            .WithMessage("Only letters, numbers, spaces, and - _ . , are allowed.");
        }
    }
}
