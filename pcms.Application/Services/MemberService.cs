using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using pcms.Application.Dto;
using pcms.Application.Validation;
using pcms.Domain.Entities;
using pcms.Domain.Enums;
using pcms.Domain.Interfaces;
using pcms.Infra;

namespace pcms.Application.Services
{
    public class MemberService : GenericRepository<Member>, IMemberService
    {
        private readonly IUnitOfWorkRepo _unitOfWorkRepo;
        private readonly IMapper _mapper;
        public MemberService(IUnitOfWorkRepo unitOfWorkRepo, AppDBContext context, ModelValidationService validationService) : base(context)
        {
            _unitOfWorkRepo = unitOfWorkRepo;
            _mapper = MappingConfig.PcmsMapConfig().CreateMapper();
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

        public async Task<string> GetTotalContributions(string memberId)
        {
            try
            {
                var member = GetByIdAsync(memberId);
                if (member == null) { return "Member not found"; }
                return (await _unitOfWorkRepo.Members.GetTotalContributionsAsync(memberId)).ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GenerateStatement(string memberId)
        {
            var member = GetByIdAsync(memberId);
            if (member == null) { return "Member not found"; }

            return await _unitOfWorkRepo.Members.GenerateStatementAsync(memberId);
        }

        public async Task<string> DeleteMemberRecord(string memberId)
        {
            var member = await GetByIdAsync(memberId);
            if (member == null) { return "Member not found"; }
            member.IsDeleted = true;
            Update(member);
            return (await SaveChangesAsync()).ToString();

            //return await  _unitOfWorkRepo.Members.GenerateStatementAsync(memberId);

        }

        public async Task<bool> AddNewMember(MemberDto memberDto)
        {
            var member = _mapper.Map<Member>(memberDto);
            var result = await AddAsync(member)? true : false;
            return result;
        }
    }
}
