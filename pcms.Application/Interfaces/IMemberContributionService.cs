namespace pcms.Application.Interfaces
{
    public interface IMemberContributionService
    {
        Task<ApiResponse<string>> GenerateStatement(string memberId);
        Task<ApiResponse<decimal>> GetTotalContributionsAsync(string memberId);
    }
}