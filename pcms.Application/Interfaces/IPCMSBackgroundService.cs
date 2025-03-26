using pcms.Application.Dto;
using pcms.Domain.Entities;

namespace pcms.Application.Interfaces
{
    public interface IPCMSBackgroundService
    {
        Task UpdateBenefitEligibility(List<Member> members);
       // Task ValidateLastMemberContribution(string memberId);
        Task ValidateMemberContributions();
        Task<ApiResponse<string>> ValidateContribution(string contributionId);
        Task CalculateContributionInterest(List<MemberDto> members);
    }
}