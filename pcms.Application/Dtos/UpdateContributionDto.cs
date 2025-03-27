using pcms.Domain.Enums;

namespace pcms.Application.Dto
{
    public class UpdateContributionDto
    {
        public string ContributionId { get; set; }

        public decimal Amount { get; set; }
        public ContributionType Type { get; set; }
       
        public DateTime ContributionDate { get; set; } = DateTime.Now;

        public int MonthForContribution { get; set; }
        public int YearForContribution { get; set; }
    }
}
