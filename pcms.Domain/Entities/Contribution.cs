using pcms.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pcms.Domain.Entities
{
    public class Contribution
    {
        [Key]
        public string ContributionId { get; set; } = Guid.NewGuid().ToString();
        public string MemberId { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }
        public ContributionType Type { get; set; }
        public DateTime ContributionDate { get; set; }
        public bool IsProcessed { get; set; } = false;
        public string? status { get; set; }
        public string? Remarks { get; set; }
        public int MonthForContribution { get; set; }
        public int YearForContribution { get; set; }
        public bool IsValid { get; set; } = true;
        public decimal? CumulativeContribution { get; set; }
        public decimal? CumulativeIntrestAmount { get; set; }
        public decimal? TotalCumulative { get; set; }
        public int EntryNumber { get; set;}
        // public Member Member { get; set; }
    }
}
