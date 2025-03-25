using pcms.Application.Interfaces;
using pcms.Domain.Interfaces;

namespace pcms.Application.Services
{
    public class MemberContributionService : IMemberContributionService
    {
        private readonly IUnitOfWorkRepo _unitOfWorkRepo;
        public MemberContributionService(IUnitOfWorkRepo unitOfWorkRepo)
        {
            _unitOfWorkRepo = unitOfWorkRepo;
        }

        public async Task<ApiResponse<decimal>> GetTotalContributionsAsync(string memberId)
        {
            var member = await _unitOfWorkRepo.Members.GetMemberWithContributions(memberId);
            var totalContributionAmount = member.Contributions.Where(c => c.IsValid && c.IsProcessed).Sum(c => c.Amount);
            var res = new ApiResponse<decimal>
            {
                Data = totalContributionAmount,
                ResponseCode = "00",
                ResponseMessage = $"Success"
            };
            return res;
        }
       
        public async Task<ApiResponse<string>> GenerateStatement(string memberId)
        {
            var member = await _unitOfWorkRepo.Members.GetMemberWithContributions(memberId);
            var totalContributionAmount = member.Contributions.Where(c => c.IsValid && c.IsProcessed).Sum(c => c.Amount);
            var res = new ApiResponse<string>
            {
                Data = $"Statement for {member.Name}: Total Contributions = {totalContributionAmount:C}",
                ResponseCode = "00",
                ResponseMessage = "Success"
            };
            return res;
        }
    }
}
