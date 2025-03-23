using pcms.Domain.Entities;
using pcms.Domain.Enums;
using pcms.Domain.Interfaces;

namespace pcms.Application.Services
{
    public class PCMSBackgroundService
    {
        private readonly IUnitOfWorkRepo _unitOfWorkRepo;
        public PCMSBackgroundService()
        {
                
        }
        public async Task UpdateBenefitEligibility()
        {
            var list = await _unitOfWorkRepo.Members.GetAllMembers();
            foreach (var member in list)
            {
                var lastContributionDate = member.Contributions.Max(x=>x.ContributionDate);
                if (lastContributionDate != null)
                {
                    if ((lastContributionDate.Month - member.RegistrationDate.Month) >= 6)
                    {
                        member.IsEligibleForBenefit = true;
                        _unitOfWorkRepo.Members.UpdateMember(member);
                    }
                }
            }
            await _unitOfWorkRepo.CompleteAsync();
        }

        public async Task ValidateMemberContribution(string contributionId)
        {
            var contribution = await _unitOfWorkRepo.Contributions.GetContribution(contributionId);
            bool IsValidTransaction = true;
            var member = await _unitOfWorkRepo.Members.GetMember(contribution.MemberId);
            int currentMonth = DateTime.Now.Month; // Get the current month

            var duplicate = member.Contributions
                    .FirstOrDefault(c => c.YearForContribution == contribution.YearForContribution && c.MonthForContribution == contribution.MonthForContribution && c.Type == ContributionType.Monthly);
            if (duplicate != null)
            {
                contribution.IsValid = false;
                contribution.Remarks = "Duplicate contribution";
                IsValidTransaction = false;
            }
           

            if (IsValidTransaction)
            {
                if(contribution.Amount <= 0)
                {
                    IsValidTransaction = false;
                    contribution.IsValid = false;
                    contribution.Remarks = "Invalid amount of 0";
                }
            }

            await _unitOfWorkRepo.CompleteAsync();
        }
        public async Task ValidateMemberContributions()
        {
            var list = await _unitOfWorkRepo.Members.GetAllMembers();
            int currentMonth = DateTime.Now.Month; // Get the current month
            foreach (var member in list)
            {
               var duplicates = member.Contributions
                    .Where(c => c.YearForContribution == DateTime.Now.Year)
                    .GroupBy(c => c.MonthForContribution)
                    .Where(g => g.Count() > 1)// Find months with more than one contribution
                    .Select(g => g.Key) // Get the month numbers
                    .ToList();



                HashSet<int> contributedMonths = 
                    new HashSet<int>(member.Contributions
                    .Where(c => c.YearForContribution == DateTime.Now.Year && c.Type == ContributionType.Monthly)
                    .Select(c => c.MonthForContribution));

                List<int> missingMonths = new List<int>();


                // Check for missing months from January to the current month and current year
                for (int month = 1; month <= currentMonth; month++)
                {
                    if (!contributedMonths.Contains(month))
                    {
                        missingMonths.Add(month);
                    }
                }

                // return missingMonths; // Returns a list of months where no contributions were made
            }
            await _unitOfWorkRepo.CompleteAsync();
        }

        public async Task UpdateMemberInterest(string memberId)
        {
            var member = await _unitOfWorkRepo.Members.GetMember(memberId);

            {
                var lastContributionDate = member.Contributions.Max(x => x.ContributionDate);
                var LastContribution = member.Contributions.FirstOrDefault(m => m.ContributionDate.Date == lastContributionDate);

                var total = await _unitOfWorkRepo.Contributions.GetTotalContributionsAsync(member.MemberId, member.RegistrationDate.Date, lastContributionDate.Date);
                LastContribution.CumulativeContribution = total + LastContribution.Amount;
                LastContribution.CumulativeIntrestAmount = LastContribution.CumulativeContribution * (10 / 100);
                LastContribution.TotalCumulative = LastContribution.CumulativeContribution + LastContribution.CumulativeIntrestAmount;
                await _unitOfWorkRepo.Contributions.UpdateContribution(LastContribution);
            }
            await _unitOfWorkRepo.CompleteAsync();
        }
    }
}
