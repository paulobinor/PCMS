using Microsoft.AspNetCore.Mvc;
using pcms.Application.Dto;
using pcms.Application.Validation;
using pcms.Domain.Entities;
using pcms.Domain.Interfaces;

namespace pcms.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class MemberController : ControllerBase
    {
        
        private readonly ILogger<MemberController> _logger;
        private readonly ModelValidationService _validationService;

        public MemberController(ILogger<MemberController> logger, IMemberService memberService, ModelValidationService validationService)
        {
            _logger = logger;
            _memberService = memberService;
            _validationService = validationService;
        }

        public IMemberService _memberService { get; }

        [HttpPost]
        [Route("AddNewMember")]
        public async Task<IActionResult> AddNewMember([FromBody] MemberDto memberDto)
        {
            var validationResult = _validationService.Validate(memberDto);
            if (!validationResult.IsValid)
            {
                return ValidationProblem(validationResult.customProblemDetail.Detail);
            }
            return Ok(await _memberService.AddNewMember(memberDto));
        }

        [HttpPost]
        [Route("AddContribution")]
        public async Task<IActionResult> AddContribution([FromBody] ContributionDto contributionDto)
        {
            var validationResult = _validationService.Validate(contributionDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.customProblemDetail.Detail);
            }
            return Ok(await _memberService.AddContribution(contributionDto));
        }

      
    }
}
