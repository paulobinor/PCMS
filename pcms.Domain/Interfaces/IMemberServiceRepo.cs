using pcms.Domain.Entities;
using pcms.Domain.Enums;

namespace pcms.Domain.Interfaces
{
    public interface IMemberServiceRepo
    {
        Task AddContributionAsync(Contribution contribution);

        Task<decimal> GetTotalContributionsAsync(string memberId, DateTime startDate, DateTime endDate);

        Task<string> GenerateStatementAsync(string memberId, DateTime startDate, DateTime endDate);
    }
}
