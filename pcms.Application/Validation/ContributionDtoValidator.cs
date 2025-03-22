using FluentValidation;
using pcms.Application.Dto;

namespace pcms.Application.Validation
{
    public class ContributionDtoValidator : AbstractValidator<ContributionDto>
    {
        public ContributionDtoValidator()
        {
            RuleFor(x => x.MemberId)
                .NotEmpty().WithMessage("MemberId not valid");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Contribution amount must be greater than zero");

            RuleFor(x => x.ContributionDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Contribution date cannot be in the future");
        }
    }
}
