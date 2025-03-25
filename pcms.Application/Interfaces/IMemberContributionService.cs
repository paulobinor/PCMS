namespace pcms.Application.Interfaces
{
    public interface IMemberContributionService
    {
        Task<ApiResponse<string>> GenerateStatement(string memberId, DateTime startDate, string DateTime);
        Task<ApiResponse<decimal>> GetTotalContributionsAsync(string memberId);
    }
}