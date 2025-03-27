using AutoMapper.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            _logger.LogInformation($"Received request to add new contribution. Payload: {JsonConvert.SerializeObject(contributionDto)}");
            var validationResult = _validationService.Validate(contributionDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.customProblemDetail.Detail);
            }
            return Created("", await _contributionService.AddContribution(contributionDto));
        }

        [HttpPut]
        [Route("Update")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateContribution([FromBody] UpdateContributionDto contributionDto)
        {
            _logger.LogInformation($"Received request to update contribution. Payload: {JsonConvert.SerializeObject(contributionDto)}");
            var validationResult = _validationService.Validate(contributionDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.customProblemDetail.Detail);
            }
            return Ok(await _contributionService.UpdateContribution(contributionDto));
        }

        [HttpGet]
        [Route("Get/{ContributionId}")]
        public async Task<IActionResult> GetContribution(string ContribuionId)
        {
            _logger.LogInformation($"Received request to get single new contribution. Payload: {JsonConvert.SerializeObject(new {ContribuionId})}");
            if (string.IsNullOrEmpty(ContribuionId))
            {
                return BadRequest(new ApiResponse<string> { ResponseCode = "01", ResponseMessage = "Invalid Id provided" });
            }
           
            return Ok(await _contributionService.GetContribution(ContribuionId));
        }

        [HttpGet]
        [Route("Member/{MemberId}")]
        public async Task<IActionResult> GetMemberContributions(string MemberId, [FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Received request to get member contributions. Payload: {JsonConvert.SerializeObject(new { MemberId })}");
            if (string.IsNullOrEmpty(MemberId))
            {
                return BadRequest(new ApiResponse<string> { ResponseCode = "01", ResponseMessage = "Invalid Id provided" });
            }
          

            var resp = await _contributionService.GetMemberContributions(MemberId);
            var pagedRes = pcms.Application.Helpers.Utilities.GetPagedList(resp.Data, pageNumber, pageSize);
            return Ok(pagedRes);
        }

        [HttpGet]
        [Route("list")]
       // [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GetAllContributions([FromQuery] string startDate = null, string endDate = null, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation($"Received request to get all contributions within a given period. Payload: {JsonConvert.SerializeObject(new { startDate, endDate })}");
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
            var resp = await _contributionService.GetContributions(fromDate, toDate);
            var pagedRes = pcms.Application.Helpers.Utilities.GetPagedList(resp.Data, pageNumber, pageSize);
            return Ok(pagedRes);
        }

        [HttpGet]
        [Route("Total/{MemberId}")]
        public async Task<IActionResult> GetTotalContributionAmount(string MemberId)
        {
            _logger.LogInformation($"Received request to get total contribution amount for a single member. Payload: {JsonConvert.SerializeObject(new { MemberId })}");
            if (string.IsNullOrEmpty(MemberId))
            {
                return BadRequest(new ApiResponse<string> { ResponseCode = "01", ResponseMessage = "Invalid Id provided" });
            }
            //var validationResult = _validationService.Validate(contributionDto);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.customProblemDetail.Detail);
            //}
            return Ok(await _memberContributionService.GetTotalContributionsAsync(MemberId));
        }

        [HttpGet]
        [Route("Statement/{MemberId}")]

       // [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GenerateStatement(string MemberId)
        {
            _logger.LogInformation($"Received request to get total contribution amount for a single member. Payload: {JsonConvert.SerializeObject(new { MemberId })}");
            if (string.IsNullOrEmpty(MemberId))
            {
                return BadRequest(new ApiResponse<string> { ResponseCode = "01", ResponseMessage = "Invalid Id provided" });
            }
            //var validationResult = _validationService.Validate(contributionDto);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.customProblemDetail.Detail);
            //}
            return Ok(await _memberContributionService.GenerateStatement(MemberId));
        }
    }
}
