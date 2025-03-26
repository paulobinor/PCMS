using pcms.Application.Dto;

namespace pcms.Application.Interfaces
{
    public interface IContributionService
    {
        Task<ApiResponse<ContributionDto>> AddContribution(AddContributionDto contributionDto);
        Task<ApiResponse<ContributionDto>> GetContribution(string contributionId);
        Task<ApiResponse<List<ContributionDto>>> GetContributions(DateTime startDate, DateTime endDate);
        Task<ApiResponse<ContributionDto>> UpdateContribution(UpdateContributionDto contributionDto);
        Task<ApiResponse<List<ContributionDto>>> GetMemberContributions(string MemberId);
    }
}