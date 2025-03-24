namespace pcms.Application.Interfaces
{
    public interface IPCMSBackgroundService
    {
        Task UpdateBenefitEligibility();
        Task UpdateMemberInterest(string memberId);
        Task ValidateMemberContribution(string contributionId);
        Task ValidateMemberContributions();
    }
}