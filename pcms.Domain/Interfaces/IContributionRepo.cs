using pcms.Domain.Entities;

namespace pcms.Domain.Interfaces
{
    public interface IContributionRepo
    {
        Task AddContributionAsync(Contribution contribution);
        Task<List<Contribution>> GetMemberContributions(string memberId);
        Task<decimal> GetTotalContributionsAsync(string memberId, DateTime startDate, DateTime endDate);
        Task<string> GenerateStatementAsync(string memberId, DateTime startDate, DateTime endDate);
        Task<Contribution> GetContribution(string contributionId);
        Task UpdateContribution(Contribution lastContribution);
    }
}
