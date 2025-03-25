using FluentValidation;
using pcms.Domain.Entities;

namespace pcms.Application.Validation
{
    public class UserLoginDtoValidator : AbstractValidator<UserLogin>
    {
        public UserLoginDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
