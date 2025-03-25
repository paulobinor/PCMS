using pcms.Application;
using pcms.Application.Dto;
using pcms.Domain.Entities;
using pcms.Domain.Enums;

namespace pcms.Domain.Interfaces
{
    public interface IMemberService 
    {
      //  Task<int> AddContribution(ContributionDto contribution);
        //Task<string> GetTotalContributions(string memberId, DateTime startDate, DateTime endDate);
        //Task<string> GenerateStatement(string memberId, DateTime startDate, DateTime endDate);
        Task<ApiResponse<string>> DeleteMemberRecord(string memberId);
        Task<ApiResponse<string>> AddNewMember(MemberDto memberDto);
        Task<ApiResponse<string>> UpdateMember(MemberDto memberDto);
       // Task<ApiResponse<decimal>> GetTotalContributionsAsync(string memberId);
        Task<ApiResponse<MemberDto>> GetMember(string memberId);
    }

}
