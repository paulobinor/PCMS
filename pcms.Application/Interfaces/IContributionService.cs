using pcms.Application.Dto;

namespace pcms.Application.Interfaces
{
    public interface IContributionService
    {
        Task<ApiResponse<ContributionDto>> AddContribution(ContributionDto contributionDto);
        Task<ApiResponse<ContributionDto>> GetContribution(string contributionId);
        Task<ApiResponse<List<ContributionDto>>> GetContributions(DateTime startDate, DateTime endDate);
        Task<ApiResponse<ContributionDto>> UpdateContribution(ContributionDto contributionDto);
    }
}