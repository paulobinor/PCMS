using Microsoft.EntityFrameworkCore;
using pcms.Domain.Entities;
using pcms.Domain.Interfaces;

namespace pcms.Infra
{
   //public class BenefitInfoRepo : GenericRepository<BenefitInfo>
   // {
   //     public BenefitInfoRepo(AppDBContext context): base(context)
   //     {
   //     }

   // }

    public class ContributionRepo : GenericRepository<Contribution>, IContributionRepo
    {
        public AppDBContext _context { get; }
        public ContributionRepo(AppDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Contribution>> GetMemberContributions(string memberId)
        {
            return (await GetAllAsync())
                .Where(c => c.MemberId == memberId).ToList();
        }

        public async Task AddContributionAsync(Contribution contribution)
        {
            try
            {
                var member = await _context.Members.FindAsync(contribution.MemberId);
                if (member == null)
                    throw new Exception("Member not found.");

                contribution.EntryNumber = (await _context.Contributions.Where(m=>m.MemberId == contribution.MemberId).CountAsync()) + 1;
                await _context.Contributions.AddAsync(contribution);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

       

        public async Task<Contribution> GetContribution(string contributionId)
        {
            return await GetByIdAsync(contributionId);
        }
        public async Task UpdateContribution(Contribution contribution)
        {
            try
            {
                Update(contribution);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Contribution>> GetContributions(DateTime startDate, DateTime endDate)
        {
            return (await GetAllAsync()).Where(m=>m.ContributionDate >= startDate && m.ContributionDate <= endDate).ToList();
        }
    }
}
