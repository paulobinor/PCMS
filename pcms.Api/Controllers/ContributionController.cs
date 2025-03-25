using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pcms.Application;
using pcms.Application.Dto;
using pcms.Application.Interfaces;
using pcms.Application.Services;
using pcms.Application.Validation;
using pcms.Domain.Entities;
using pcms.Domain.Interfaces;

namespace pcms.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/Contributions")]
    public class ContributionController : ControllerBase
    {
        
        private readonly ILogger<ContributionController> _logger;
        private readonly ModelValidationService _validationService;
        public readonly IContributionService _contributionService;
        public readonly IMemberContributionService _memberContributionService;

        public ContributionController(ILogger<ContributionController> logger, ModelValidationService validationService, IContributionService contributionService, IMemberContributionService memberContributionService)
        {
            _logger = logger;
            _validationService = validationService;
            _contributionService = contributionService;
            _memberContributionService = memberContributionService;
        }


        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddContribution([FromBody] AddContributionDto contributionDto)
        {
            var validationResult = _validationService.Validate(contributionDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.customProblemDetail.Detail);
            }
            return Created("", await _contributionService.AddContribution(contributionDto));
        }

        [HttpPut]
        [Route("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateContribution([FromBody] ContributionDto contributionDto)
        {
            var validationResult = _validationService.Validate(contributionDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.customProblemDetail.Detail);
            }
            return Ok(await _contributionService.UpdateContribution(contributionDto));
        }

        [HttpGet]
        [Route("GetContribution/{ContributionId}")]
        public async Task<IActionResult> GetContribution(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest(new ApiResponse<string> { ResponseCode = "01", ResponseMessage = "Invalid Id provided" });
            }
            //var validationResult = _validationService.Validate(contributionDto);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.customProblemDetail.Detail);
            //}
            return Ok(await _contributionService.GetContribution(Id));
        }

        [HttpGet]
        [Route("GetMemberContributions/{MemberId}")]
        public async Task<IActionResult> GetMemberContributions(string MemberId)
        {
            //if (string.IsNullOrEmpty(Id))
            //{
            //    return BadRequest(new ApiResponse<string> { ResponseCode = "25", ResponseMessage = "Invalid Id provided" });
            //}
            //var validationResult = _validationService.Validate(contributionDto);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.customProblemDetail.Detail);
            //}
            return Ok(await _contributionService.GetMemberContributions(MemberId));
        }

        [HttpGet]
        [Route("GetContributionsList")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GetAllContributions([FromQuery] string startDate, [FromQuery] string endDate = null)
        {
            DateTime fromDate = new DateTime(DateTime.Now.Year,1,1);
            DateTime toDate = DateTime.Now;
            if (startDate != null)
            {
                if (DateTime.TryParse(startDate, out _))
                {
                    fromDate = Convert.ToDateTime(startDate);
                }   
            }
            if (endDate != null) 
            {
                if (DateTime.TryParse(endDate, out _))
                {
                    toDate = Convert.ToDateTime(endDate);
                }
            }
            //if (string.IsNullOrEmpty(Id))
            //{
            //    return BadRequest(new ApiResponse<string> { ResponseCode = "25", ResponseMessage = "Invalid Id provided" });
            //}
            //var validationResult = _validationService.Validate(contributionDto);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.customProblemDetail.Detail);
            //}
            return Ok(await _contributionService.GetContributions(fromDate, toDate));
        }

        [HttpGet]
        [Route("Reports/{MemberId}/GetTotalContribution")]
        public async Task<IActionResult> GetTotalContributionAmount(string MemberId)
        {
            //if (string.IsNullOrEmpty(Id))
            //{
            //    return BadRequest(new ApiResponse<string> { ResponseCode = "25", ResponseMessage = "Invalid Id provided" });
            //}
            //var validationResult = _validationService.Validate(contributionDto);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.customProblemDetail.Detail);
            //}
            return Ok(await _memberContributionService.GetTotalContributionsAsync(MemberId));
        }

        [HttpGet]
        [Route("Reports/{MemberId}/GenerateStatement")]

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GenerateStatement(string MemberId)
        {
            //if (string.IsNullOrEmpty(Id))
            //{
            //    return BadRequest(new ApiResponse<string> { ResponseCode = "25", ResponseMessage = "Invalid Id provided" });
            //}
            //var validationResult = _validationService.Validate(contributionDto);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.customProblemDetail.Detail);
            //}
            return Ok(await _memberContributionService.GenerateStatement(MemberId));
        }
    }
}
