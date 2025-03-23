using System.ComponentModel.DataAnnotations;

namespace pcms.Domain.Entities
{
    public class BenefitInfo
    {
        [Key]
        public string BenefitId { get;  set; }
        public string MemberId { get;  set; }

        public int TotalContributions { get; set; }
        public int interestRate { get; set; }
        public decimal TotalContributionAmount { get; set; }
        public decimal TotalBalanceWithInterst 
        {
            get
            {
                return (TotalContributionAmount*(interestRate/100) + TotalContributionAmount);
            }
        }
        public decimal CurrentInterestAmount
        {
            get
            {
                return TotalContributionAmount * (interestRate / 100);
            }
        }
    }
}
