using Microsoft.AspNetCore.Mvc;
using pcms.Application;
using pcms.Application.Dto;
using pcms.Application.Validation;
using pcms.Domain.Entities;
using pcms.Domain.Interfaces;

namespace pcms.Api.Controllers
{
    [ApiController]
    [Route("api/Members")]
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
            _logger.LogInformation("Received request to add new member");
            var validationResult = _validationService.Validate(memberDto);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Invalid request parameters: {validationResult.customProblemDetail.Detail}");
                return ValidationProblem(validationResult.customProblemDetail.Detail);
            }
            return Ok(await _memberService.AddNewMember(memberDto));
        }

        [HttpGet]
        [Route("Get/{Id}")]
        public async Task<IActionResult> GetMember(string Id)
        {
            _logger.LogInformation("Received request to get member");
            //var validationResult = _validationService.Validate(Id);
            //if (!validationResult.IsValid)
            //{
            //    return ValidationProblem(validationResult.customProblemDetail.Detail);
            //}
            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest(new ApiResponse<string> { ResponseCode = "01", ResponseMessage = "Invalid Id provided" });
            }
            return Ok(await _memberService.GetMember(Id));
        }

        [HttpPost]
        [Route("Remove")]
        public async Task<IActionResult> RemoveMember([FromBody] string MemberId)
        {
            _logger.LogInformation("Received request to remove member");
            //var validationResult = _validationService.Validate(Id);
            //if (!validationResult.IsValid)
            //{
            //    return ValidationProblem(validationResult.customProblemDetail.Detail);
            //}
            if (string.IsNullOrEmpty(MemberId))
            {
                _logger.LogInformation("Invalid Id provided.");
                return BadRequest(new ApiResponse<string> { ResponseCode = "01", ResponseMessage = "Invalid Id provided" });
            }
            return Ok(await _memberService.DeleteMemberRecord(MemberId));
        }
    }
}
