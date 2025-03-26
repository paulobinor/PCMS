using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pcms.Domain.Entities;
using pcms.Domain.Enums;
using pcms.Domain.Interfaces;

namespace pcms.Infra
{
    public class MemberServiceRepo : GenericRepository<Member>, IMemberServiceRepo
    {
        public AppDBContext _context { get; }
        private readonly ILogger<MemberServiceRepo> _logger;

        public MemberServiceRepo(AppDBContext context, ILogger<MemberServiceRepo> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddMember(Member member)
        {
            try
            {
                await AddAsync(member);
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
                var exists = await GetByIdAsync(member.MemberId);
               
                Update(member);
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
               
                member.IsDeleted = true;
                Update(member);
                //return "Success";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Member>> GetAllMembers()
        {
            try
            {
                return (await GetAllAsync()).Where(m => !m.IsDeleted).ToList();
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
                var member = await GetByIdAsync(MemberId);
                if (member == null)
                {
                    throw new Exception($"Member with Id:{MemberId} Not Found! or does not exist");
                    //"log Not Found";
                }
                return member;
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
