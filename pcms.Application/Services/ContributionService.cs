using AutoMapper;
using AutoMapper.Execution;
using Azure;
using Hangfire;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using pcms.Application.Dto;
using pcms.Application.Interfaces;
using pcms.Domain.Entities;
using pcms.Domain.Interfaces;

namespace pcms.Application.Services
{
    public class ContributionService : IContributionService
    {
        private readonly IUnitOfWorkRepo _unitOfWorkRepo;
        private readonly IPCMSBackgroundService _ipcmsBackgroundService;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ContributionService> _logger;

        public ContributionService(IUnitOfWorkRepo unitOfWorkRepo, ICacheService cacheService, IPCMSBackgroundService ipcmsBackgroundService, ILogger<ContributionService> logger)
        {
            _unitOfWorkRepo = unitOfWorkRepo;
            _mapper = MappingConfig.PcmsMapConfig().CreateMapper();
            _cacheService = cacheService;
            _ipcmsBackgroundService = ipcmsBackgroundService;
            _logger = logger;
        }

        public async Task<ApiResponse<ContributionDto>> AddContribution(ContributionDto contributionDto)
        {
            var response = new ApiResponse<ContributionDto>();
            try
            {
                var member = await _unitOfWorkRepo.Members.GetMember(contributionDto.MemberId);
                if (member == null)
                    throw new Exception("Member not found.");

                var contribution = _mapper.Map<Contribution>(contributionDto);
                await _unitOfWorkRepo.Contributions.AddContributionAsync(contribution);
                var res = await _unitOfWorkRepo.CompleteAsync();
                if (res > 0)
                {
                    var memberContribution = (await _unitOfWorkRepo.Contributions.GetContributions(contribution.ContributionDate, contributionDto.ContributionDate)).FirstOrDefault(m=>m.Amount == contribution.Amount && m.MemberId == contribution.MemberId);
                    if (memberContribution != null)
                    {
                       string ValidateContributionJobId = BackgroundJob.Enqueue(() => _ipcmsBackgroundService.ValidateMemberContribution(memberContribution.ContributionId));
                        
                       string intrestCalcJobId = BackgroundJob.ContinueJobWith(ValidateContributionJobId, () => _ipcmsBackgroundService.UpdateMemberInterest(memberContribution.MemberId));
                    }
                    response.Data = _mapper.Map<ContributionDto>(memberContribution);
                    response.ResponseMessage = "Success";
                    response.ResponseCode = "00";
                    return response;
                }
                response.ResponseMessage = "failed to add new contribution. See logs for details";
                response.ResponseCode = "01";
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResponse<ContributionDto>> GetContribution(string contributionId)
        {
            try
            {
                var response = new ApiResponse<ContributionDto>();
                ContributionDto? contributionDto = default;
                var contributionJson = await _cacheService.GetDataAsync(contributionId);
                if (!string.IsNullOrEmpty(contributionJson))
                {
                    contributionDto = JsonConvert.DeserializeObject<ContributionDto>(contributionJson);
                }
                else
                {
                    var result = await _unitOfWorkRepo.Contributions.GetContribution(contributionId);
                    contributionDto = _mapper.Map<ContributionDto>(result);
                }
                if (contributionDto != null)
                {
                    response.Data = contributionDto;
                    response.ResponseMessage = "Success";
                    response.ResponseCode = "00";
                    _cacheService.SetData(contributionId, JsonConvert.SerializeObject(contributionDto), 36000);
                }
                else
                {
                    response.ResponseMessage = "failed to retreive Member details";
                    response.ResponseCode = "01";
                }

                return response;


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResponse<ContributionDto>> UpdateContribution(ContributionDto contributionDto)
        {
            var response = new ApiResponse<ContributionDto>();
            try
            {
                var contribution = _mapper.Map<Contribution>(contributionDto);
                 await _unitOfWorkRepo.Contributions.UpdateContribution(contribution);
                var result = await _unitOfWorkRepo.CompleteAsync();
                if (result > 0)
                {
                    response.ResponseMessage = "Success";
                    response.ResponseCode = "00";
                }
                else
                {
                    response.ResponseMessage = "failed to update contribution details";
                    response.ResponseCode = "01";
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error message {ex.Message}");
                response.ResponseCode = "96";
                response.ResponseMessage = "We experienced an internal server error";
            }

            return response;
        }

        public async Task<ApiResponse<List<ContributionDto>>> GetContributions(DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = new ApiResponse<List<ContributionDto>>();

                var result = await _unitOfWorkRepo.Contributions.GetContributions(startDate, endDate);

                if (result != null)
                {
                    response.Data = _mapper.Map<List<ContributionDto>>(result);
                    response.ResponseMessage = "Success";
                    response.ResponseCode = "00";
                }
                else
                {
                    response.ResponseMessage = "failed to retreive Member details";
                    response.ResponseCode = "01";
                }

                return response;


            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
