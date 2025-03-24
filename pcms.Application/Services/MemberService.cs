using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using pcms.Application.Dto;
using pcms.Application.Interfaces;
using pcms.Application.Validation;
using pcms.Domain.Entities;
using pcms.Domain.Enums;
using pcms.Domain.Interfaces;
using pcms.Infra;

namespace pcms.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWorkRepo _unitOfWorkRepo;
        private readonly IMapper _mapper;
        private IValidationService _validationService;
        public MemberService(IUnitOfWorkRepo unitOfWorkRepo, IValidationService validationService)
        {
            _unitOfWorkRepo = unitOfWorkRepo;
            _mapper = MappingConfig.PcmsMapConfig().CreateMapper();
            _validationService = validationService;
        }


        public async Task<ApiResponse<string>> DeleteMemberRecord(string memberId)
        {
            var updateMember = await _unitOfWorkRepo.Members.RemoveMember(memberId);
            var result = await _unitOfWorkRepo.CompleteAsync();
            if (result > 0)
            {
                return new ApiResponse<string> { Data = "Success", ResponseCode = "00", ResponseMessage = "Record successfully deleted"};
            }
            return new ApiResponse<string> { Data = "", ResponseCode = "01", ResponseMessage = "We could not execute the request" };
        }

        public async Task<ApiResponse<string>> AddNewMember(MemberDto memberDto)
        {
            var validateResult = await _validationService.ValidateEmployer(memberDto.Employer);
            if (validateResult != "00")
            {
                return new ApiResponse<string> { Data = "", ResponseCode = "01", ResponseMessage = "Employer verification failed!" };
            }
            var member = _mapper.Map<Member>(memberDto);
             await _unitOfWorkRepo.Members.AddMember(member);
           var result = await _unitOfWorkRepo.CompleteAsync();

            return result > 0 ? new ApiResponse<string> { Data = "Success", ResponseCode = "00", ResponseMessage = "member successfully created" } : new ApiResponse<string> { Data = "", ResponseCode = "01", ResponseMessage = "We could not execute the request!" };
        }

        public async Task<ApiResponse<string>> UpdateMember(MemberDto memberDto)
        {
            var member = _mapper.Map<Member>(memberDto);
            var result =  await _unitOfWorkRepo.Members.UpdateMember(member).ContinueWith((n) => _unitOfWorkRepo.CompleteAsync());
            return result.Result > 0 ? new ApiResponse<string> { Data = "Success", ResponseCode = "00", ResponseMessage = "Record successfully deleted" } : new ApiResponse<string> { Data = "", ResponseCode = "01", ResponseMessage = "We could not execute the request!" };
        }
        public async Task<ApiResponse<string>> GenerateStatement(string memberId, DateTime startDate, string DateTime)
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
        public async Task<decimal> GetTotalContributionsAsync(string memberId)
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
