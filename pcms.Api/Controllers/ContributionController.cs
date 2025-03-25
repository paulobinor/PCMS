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
    [Route("api/Contributions")]
    public class ContributionController : ControllerBase
    {
        
        private readonly ILogger<ContributionController> _logger;
        private readonly ModelValidationService _validationService;
        public readonly IContributionService _contributionService;

        public ContributionController(ILogger<ContributionController> logger, ModelValidationService validationService, IContributionService contributionService)
        {
            _logger = logger;
            _validationService = validationService;
            _contributionService = contributionService;
        }


        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddContribution([FromBody] ContributionDto contributionDto)
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
        [Route("Get/{id}")]
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
        [Route("Get")]
        public async Task<IActionResult> GetContributions([FromQuery]DateTime startDate, [FromQuery] DateTime endDate)
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
            return Ok(await _contributionService.GetContributions(startDate, endDate));
        }
    }
}
