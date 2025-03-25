using FluentValidation;
using pcms.Domain.Entities;

namespace pcms.Application.Validation
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUser>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email not valid");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");

            RuleFor(x => x.UserRole)
                .NotEmpty().WithMessage("Role is required");
        }
    }
}
