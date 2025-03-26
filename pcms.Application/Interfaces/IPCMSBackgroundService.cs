namespace pcms.Application.Interfaces
{
    public interface IPCMSBackgroundService
    {
        Task UpdateBenefitEligibility();
        Task UpdateMemberInterest(string memberId);
        Task ValidateLastMemberContribution(string memberId);
        Task ValidateMemberContributions();
        Task UpdateAllMemberInterest();
        Task<ApiResponse<string>> ValidateContribution(string contributionId);
    }
}