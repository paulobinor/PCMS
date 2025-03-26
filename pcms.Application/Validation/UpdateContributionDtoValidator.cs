using FluentValidation;
using pcms.Application.Dto;

namespace pcms.Application.Validation
{
    public class UpdateContributionDtoValidator : AbstractValidator<UpdateContributionDto>
    {
        public UpdateContributionDtoValidator()
        {

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Contribution amount must be greater than zero");

            RuleFor(x => x.ContributionDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Contribution date cannot be in the future");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid contribution type. Allowed values: Monthly = 0, Voluntary = 1");

            RuleFor(x => x.MonthForContribution)
                .LessThanOrEqualTo(12).WithMessage("Invalid contribution month. must be equal to or between 1 and 12")
                .GreaterThan(0).WithMessage("Invalid contribution month. must be greater than 0");

            RuleFor(x => x.YearForContribution)
                .LessThanOrEqualTo(DateTime.Now.Year).WithMessage("Year cannot be in the future")
                .GreaterThan(0).WithMessage("Invalid contribution Year. Must be greater than 0");
        }
    }
}
