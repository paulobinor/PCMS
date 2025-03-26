using AutoMapper.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pcms.Application;
using pcms.Application.Dto;
using pcms.Application.Interfaces;
using pcms.Application.Validation;
using pcms.Domain.Entities;
using pcms.Domain.Interfaces;

namespace pcms.Api.Controllers
{
    [ApiController]
    [Route("api/")]
    [Authorize(Roles = "Admin")]
    public class BackgroundProcessesController : ControllerBase
    {
        private readonly ILogger<MemberController> _logger;
        private readonly ModelValidationService _validationService;
        private readonly IPCMSBackgroundService _pCMSBackgroundService;
        private readonly IContributionService _contributionService;

        public BackgroundProcessesController(ILogger<MemberController> logger, IMemberService memberService, ModelValidationService validationService, IContributionService contributionService, IPCMSBackgroundService pCMSBackgroundService)
        {
            _logger = logger;
            _memberService = memberService;
            _validationService = validationService;
            _contributionService = contributionService;
            _pCMSBackgroundService = pCMSBackgroundService;
        }

        public IMemberService _memberService { get; }

        [HttpPost]
        [Route("Jobs/{MemberId}/UpdateInterest")]
        public async Task<IActionResult> UpdateMemberInterest(string MemberId)
        {
            _logger.LogInformation($"Received request to Update member interest. MemberId: {MemberId}");

            //var validationResult = _validationService.Validate(memberDto);
            //if (!validationResult.IsValid)
            //{
            //    return ValidationProblem(validationResult.customProblemDetail.Detail);
            //}
            var member = await _memberService.GetMember(MemberId);
            if (member.ResponseCode == "00")
            {
                await _pCMSBackgroundService.UpdateMemberInterest(MemberId);
                return Ok();
            }
            return BadRequest(new {member.ResponseMessage, member.ResponseCode});
        }

        [HttpGet]
        [Route("Jobs/{ContributionId}/Validate")]
        public async Task<IActionResult> ValidateMemberContribution(string ContributionId)
        {
            _logger.LogInformation($"Received request to validate member contribution. ContributionId: {ContributionId}");
            //var validationResult = _validationService.Validate(Id);
            //if (!validationResult.IsValid)
            //{
            //    return ValidationProblem(validationResult.customProblemDetail.Detail);
            //}
            var member = await _contributionService.GetContribution(ContributionId);
            if (member.ResponseCode == "00")
            {
                var response = await _pCMSBackgroundService.ValidateContribution(ContributionId);
                return Ok(response);
            }
            return BadRequest(new { member.ResponseMessage, member.ResponseCode });
        }
    }
}
