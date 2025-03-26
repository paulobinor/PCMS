using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
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
        private readonly ICacheService _cacheService;
        private readonly ILogger<MemberService> _logger;
        public MemberService(IUnitOfWorkRepo unitOfWorkRepo, IValidationService validationService, ICacheService cacheService, ILogger<MemberService> logger)
        {
            _unitOfWorkRepo = unitOfWorkRepo;
            _mapper = MappingConfig.PcmsMapConfig().CreateMapper();
            _validationService = validationService;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> DeleteMemberRecord(string memberId)
        {
            await _unitOfWorkRepo.Members.RemoveMember(memberId);
            var result = await _unitOfWorkRepo.CompleteAsync();
            if (result > 0)
            {
                _logger.LogInformation("Member details have been deleted successfully");
                return new ApiResponse<string> { Data = "Success", ResponseCode = "00", ResponseMessage = "Record successfully deleted"};
            }

            _logger.LogError("failed to delete member details.");
            return new ApiResponse<string> { Data = "", ResponseCode = "01", ResponseMessage = "We could not execute the request" };
        }

        public async Task<ApiResponse<string>> AddNewMember(AddMemberDto memberDto)
        {
            var member = _mapper.Map<Domain.Entities.Member>(memberDto);
             await _unitOfWorkRepo.Members.AddMember(member);
           var result = await _unitOfWorkRepo.CompleteAsync();
            if (result > 0)
            {
                _logger.LogInformation("Member details have been created successfully");
                return new ApiResponse<string> { Data = "Success", ResponseCode = "00", ResponseMessage = "member successfully created" };
            }
            _logger.LogError("failed to add new member details.");
            return new ApiResponse<string> { Data = "", ResponseCode = "01", ResponseMessage = "We could not execute the request!" };
        }

        public async Task<ApiResponse<string>> UpdateMember(MemberDto memberDto)
        {
            var response = new ApiResponse<string>();

            var updateMember = await _unitOfWorkRepo.Members.GetMember(memberDto.MemberId);
            if (updateMember == null)
            {
                response.ResponseMessage = "Member not found or does not exist";
                response.ResponseCode = "01";
                return response;
            }
            updateMember.Employer = memberDto.Employer;
            updateMember.RSAPin = memberDto.RSAPin;
            updateMember.Email = memberDto.Email;
            updateMember.DateOfBirth = memberDto.DateOfBirth;
            updateMember.Phone = memberDto.Phone;
            updateMember.IsActive = memberDto.IsActive;   
            updateMember.Name = memberDto.Name;
            updateMember.IsEligibleForBenefit = memberDto.IsLegibleforBenefit;
            updateMember.RegistrationDate = memberDto.RegistrationDate;

            //var member = _mapper.Map<Domain.Entities.Member>(memberDto);

            var result =  await _unitOfWorkRepo.Members.UpdateMember(updateMember).ContinueWith((n) => _unitOfWorkRepo.CompleteAsync());

            _cacheService.SetData(memberDto.MemberId, JsonConvert.SerializeObject(memberDto), 36000);
            if (result.Result > 0)
            {
                _logger.LogInformation("Member details have been updated successfully");
                return new ApiResponse<string> { Data = "Success", ResponseCode = "00", ResponseMessage = "Record successfully updated" };
            }

            _logger.LogError("failed to update member details.");
            return new ApiResponse<string> { Data = "", ResponseCode = "01", ResponseMessage = "We could not execute the request!" };
        }

        public async Task<ApiResponse<MemberDto>> GetMember(string memberId)
        {
            var response = new ApiResponse<MemberDto>();
            MemberDto? memberDto = default;
            var memberJson = await _cacheService.GetDataAsync(memberId);
            if (!string.IsNullOrEmpty(memberJson))
            {
                memberDto = JsonConvert.DeserializeObject<MemberDto>(memberJson);
            }
            else
            {
                var result = await _unitOfWorkRepo.Members.GetMember(memberId);
                memberDto = _mapper.Map<MemberDto>(result);
            }
            if (memberDto != null)
            {
                response.Data = memberDto;
                response.ResponseMessage = "Success";
                response.ResponseCode = "00";
                _cacheService.SetData(memberId, JsonConvert.SerializeObject(memberDto), 36000);

                _logger.LogInformation("Member details retrieved successfully");
            }
            else
            {
                _logger.LogInformation("Failed to retreive Member details");
                response.ResponseMessage = "failed to retreive Member details";
                response.ResponseCode = "01";
            }
            return response;
        }

        public async Task<ApiResponse<List<MemberDto>>> GetAllMembers()
        {
            var response = new ApiResponse<List<MemberDto>>();
            List<MemberDto>? membersDto = default;
            var memberJson = await _cacheService.GetDataAsync("allMembers");
            if (!string.IsNullOrEmpty(memberJson))
            {
                membersDto = JsonConvert.DeserializeObject<List<MemberDto>>(memberJson);
            }
            else
            {
                var result = await _unitOfWorkRepo.Members.GetAllMembers();
                membersDto = _mapper.Map<List<MemberDto>>(result);
            }
            if (membersDto != null)
            {
                response.Data = membersDto;
                response.ResponseMessage = "Success";
                response.ResponseCode = "00";
                _cacheService.SetData("allMembers", JsonConvert.SerializeObject(membersDto), 36000);
                _logger.LogInformation("Member details retrieved successfully");
            }
            else
            {
                _logger.LogInformation("Failed to retreive Member details");
                response.ResponseMessage = "failed to retreive Member details";
                response.ResponseCode = "01";
            }
            return response;
        }

    }
}
