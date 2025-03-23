using pcms.Application.Dto;
using pcms.Domain.Entities;
using pcms.Domain.Interfaces;

namespace pcms.Application.Services
{
    public class ContributionService
    {
        private readonly IUnitOfWorkRepo _unitOfWorkRepo;
        public ContributionService(IUnitOfWorkRepo unitOfWorkRepo)
        {
            _unitOfWorkRepo = unitOfWorkRepo;
        }
        
        public async Task<Contribution> GetContributionAsync()
        {
            var res = _unitOfWorkRepo.Contributions
        }

        public async Task<string> GetTotalContributions(string memberId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var member = GetByIdAsync(memberId);
                if (member == null) { return "Member not found"; }
                return (await _unitOfWorkRepo.Members.GetTotalContributionsAsync(memberId, startDate, endDate)).ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> AddContribution(ContributionDto contributionDto)
        {
            try
            {
                var member = await GetByIdAsync(contributionDto.MemberId);
                if (member == null)
                    throw new Exception("Member not found.");

                var contribution = _mapper.Map<Contribution>(contributionDto);
                await _unitOfWorkRepo.Members.AddContributionAsync(contribution);
                return await _unitOfWorkRepo.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<string> GenerateStatement(string memberId, DateTime startDate, DateTime endDate)
        {
            var member = GetByIdAsync(memberId);
            if (member == null) { return "Member not found"; }

            return await _unitOfWorkRepo.Members.GenerateStatementAsync(memberId, startDate, endDate);
        }
    }
}
