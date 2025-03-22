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

        public async Task AddContributionAsync(Contribution contribution)
        {
            try
            {
                var member = await _context.Members.FindAsync(contribution.MemberId);
                if (member == null)
                    throw new Exception("Member not found.");

                await _context.Contributions.AddAsync(contribution);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<decimal> GetTotalContributionsAsync(string memberId, DateTime startDate, DateTime endDate)
        {

            return await _context.Contributions
                .Where(c => c.MemberId == memberId && c.ContributionDate >= startDate.Date && c.ContributionDate <= endDate.Date)
                .SumAsync(c => c.Amount);
        }

        public async Task<string> GenerateStatementAsync(string memberId, DateTime startDate, DateTime endDate)
        {
            var member = await _context.Members
              //  .Include(m => m.Contributions)
                .FirstOrDefaultAsync(m => m.MemberId == memberId);

            //if (member == null)
            //    return "Member not found.";

            var total = await GetTotalContributionsAsync(memberId, startDate, endDate);
            return $"Statement for {member.Name}: Total Contributions = {total:C}";
        }
    }
}
