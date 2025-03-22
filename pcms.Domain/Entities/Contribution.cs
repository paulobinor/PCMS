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
       // public Member Member { get; set; }
    }
}
