using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pcms.Application;
using pcms.Application.Dto;
using pcms.Application.Validation;
using pcms.Domain.Entities;
using pcms.Domain.Interfaces;

namespace pcms.Api.Controllers
{
    [ApiController]
    [Authorize]
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
        [Route("Add")]
        public async Task<IActionResult> AddNewMember([FromBody] AddMemberDto memberDto)
        {
            _logger.LogInformation($"Received request to add new member. Payload - {JsonConvert.SerializeObject(new { memberDto })}");
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
            _logger.LogInformation($"Received request to get member. Payload - {JsonConvert.SerializeObject(new { Id })}");
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

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetAllMembers([FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("Received request to get member");
            //var validationResult = _validationService.Validate(Id);
            //if (!validationResult.IsValid)
            //{
            //    return ValidationProblem(validationResult.customProblemDetail.Detail);
            //}
            
            var resp = await _memberService.GetAllMembers();
            var pagedRes = pcms.Application.Helpers.Utilities.GetPagedList(resp.Data, pageNumber, pageSize);
            return Ok(pagedRes);
        }

        [HttpDelete]
        [Route("Remove/{MemberId}")]
        public async Task<IActionResult> RemoveMember(string MemberId)
        {
            _logger.LogInformation($"Received request to remove member. Payload - {JsonConvert.SerializeObject(new {MemberId})}");
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
