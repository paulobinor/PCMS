using pcms.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pcms.Application.Dto
{
    public class ContributionDto
    {
        public string ContributionId { get; set; } = Guid.NewGuid().ToString();
        public string MemberId { get; set; }

        public decimal Amount { get; set; }
        public ContributionType Type { get; set; } = ContributionType.Monthly;
        public DateTime ContributionDate { get; set; } = DateTime.Now;
       // public Member Member { get; set; }
    }
}
