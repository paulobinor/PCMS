using Microsoft.EntityFrameworkCore;
using pcms.Domain.Entities;
using pcms.Domain.Enums;
using pcms.Domain.Interfaces;

namespace pcms.Infra
{
    public class MemberServiceRepo : GenericRepository<Member>, IMemberServiceRepo
    {
        public AppDBContext _context { get; }

        public MemberServiceRepo(AppDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddMember(Member member)
        {
            try
            {
                await AddAsync(member);
                //if (member == null)
                //{
                //    //"log Not Found";
                //}
                //return "Success";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateMember(Member member)
        {
            try
            {
                Update(member);
                //if (member == null)
                //{
                //    //"log Not Found";
                //}
                //return "Success";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveMember(string memberId)
        {
            try
            {
                var member = await GetByIdAsync(memberId);
                if (member == null) 
                {
                    //"log Not Found";
                }
                member.IsDeleted = true;
                Update(member);
                //return "Success";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Member>> GetAllMembers()
        {
            try
            {
                return (await GetAllAsync()).ToList();
                //return "Success";
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Member> GetMember(string MemberId)
        {
            try
            {
                return await GetByIdAsync(MemberId);
                //return "Success";
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Member> GetMemberWithContributions(string MemberId)
        {
            try
            {
                return  await _context.Members.Include("Contributions").FirstOrDefaultAsync(m => m.MemberId == MemberId);
                //return "Success";
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
