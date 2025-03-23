using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using pcms.Application.Dto;
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
        public MemberService(IUnitOfWorkRepo unitOfWorkRepo, AppDBContext context, ModelValidationService validationService)
        {
            _unitOfWorkRepo = unitOfWorkRepo;
            _mapper = MappingConfig.PcmsMapConfig().CreateMapper();
        }

       
       

        public async Task<bool> DeleteMemberRecord(string memberId)
        {
            var updateMember = await _unitOfWorkRepo.Members.RemoveMember(memberId);
            var result = await _unitOfWorkRepo.CompleteAsync();
            if (result > 0)
            {
                return true;
            }
            return false;

            //return await  _unitOfWorkRepo.Members.GenerateStatementAsync(memberId);

        }

        public async Task<bool> AddNewMember(MemberDto memberDto)
        {
            var member = _mapper.Map<Member>(memberDto);
             await _unitOfWorkRepo.Members.AddMember(member);
           var result = await _unitOfWorkRepo.CompleteAsync();
            return result > 0 ? true : false ;
        }

        public async Task GenerateBenefitEligibility()
        {
            
        }
    }
}
