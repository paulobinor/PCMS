using pcms.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pcms.Application.Dto
{
    public class ContributionDto
    {
        private ContributionType contributionType;
        public string ContributionId { get; set; } = Guid.NewGuid().ToString();
        public string MemberId { get; set; }

        public decimal Amount { get; set; }
        public string Type
        {
            get 
            {
                return contributionType.ToString();
            }
            set
            {
                contributionType = (ContributionType)Enum.Parse(typeof(ContributionType), value);
            }
        }
        public DateTime ContributionDate { get; set; } = DateTime.Now;

        public bool IsProcessed { get; set; } = false;
        public string? status { get; set; }
        public string? Remarks { get; set; }
        public int MonthForContribution { get; set; }
        public int YearForContribution { get; set; }
        public bool IsValid { get; set; } = true;
        public decimal? CumulativeContribution { get; set; }
        public decimal? CumulativeIntrestAmount { get; set; }
        public decimal? TotalCumulative { get; set; }
    }
}
