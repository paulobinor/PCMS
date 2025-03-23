using pcms.Application.Dto;
using pcms.Domain.Entities;
using pcms.Domain.Enums;

namespace pcms.Domain.Interfaces
{
    public interface IMemberService 
    {
        Task<int> AddContribution(ContributionDto contribution);
        Task<string> GetTotalContributions(string memberId, DateTime startDate, DateTime endDate);
        Task<string> GenerateStatement(string memberId, DateTime startDate, DateTime endDate);
        Task<string> DeleteMemberRecord(string memberId);
        Task<bool> AddNewMember(MemberDto memberDto);
    }
}
