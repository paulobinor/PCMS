using pcms.Domain.Entities;
using pcms.Domain.Enums;
using System;

namespace pcms.Domain.Interfaces
{
    public interface IMemberServiceRepo
    {
        Task AddMember(Member member);

        Task RemoveMember(string memberId);
        Task<List<Member>> GetAllMembers();
        Task UpdateMember(Member member);
        Task<Member> GetMember(string memberId);
        Task<Member> GetMemberWithContributions(string MemberId);
    }
}
