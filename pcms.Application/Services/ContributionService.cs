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
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ContributionService(IUnitOfWorkRepo unitOfWorkRepo, ICacheService cacheService, IPCMSBackgroundService ipcmsBackgroundService, ILogger<ContributionService> logger, IBackgroundJobClient backgroundJobClient)
        {
            _unitOfWorkRepo = unitOfWorkRepo;
            _mapper = MappingConfig.PcmsMapConfig().CreateMapper();
            _cacheService = cacheService;
            _ipcmsBackgroundService = ipcmsBackgroundService;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<ApiResponse<ContributionDto>> AddContribution(AddContributionDto contributionDto)
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

                   // await _ipcmsBackgroundService.ValidateMemberContribution(contributionDto.MemberId);

                   // await _ipcmsBackgroundService.UpdateMemberInterest(contributionDto.MemberId);

                    string ValidateContributionJobId = _backgroundJobClient.Enqueue(() => _ipcmsBackgroundService.ValidateLastMemberContribution(contributionDto.MemberId));

                    string intrestCalcJobId = _backgroundJobClient.ContinueJobWith(ValidateContributionJobId, () => _ipcmsBackgroundService.UpdateMemberInterest(contributionDto.MemberId));

                    // response.Data = _mapper.Map<ContributionDto>(memberContribution);
                    response.ResponseMessage = "Success";
                    response.ResponseCode = "00";
                    _logger.LogInformation($"Contribution details created successfully for MemberId: {contributionDto.MemberId}");
                    return response;
                }
                _logger.LogError($"failed to add new contribution for MemberId{contributionDto.MemberId}");
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
                    _logger.LogInformation($"Retrieved contribution data from cache for ContributionId: {contributionId}");
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
                    _logger.LogInformation($"Retrieved contribution data successfully");
                }
                else
                {
                    response.ResponseMessage = "failed to retreive Member details";
                    response.ResponseCode = "01";
                    _logger.LogInformation($"Failed to retreive Member details");
                }

                return response;


            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while retrieving Member contributionId: {contributionId}. message: {ex.Message}");
                throw;
            }
        }

        public async Task<ApiResponse<ContributionDto>> UpdateContribution(UpdateContributionDto contributionDto)
        {
            var response = new ApiResponse<ContributionDto>();
            try
            {
                var UpdateContribution = await _unitOfWorkRepo.Contributions.GetContribution(contributionDto.ContributionId);
                if (UpdateContribution == null)
                {
                    response.ResponseMessage = "Not found";
                    response.ResponseCode = "01";
                }
                UpdateContribution.YearForContribution = contributionDto.YearForContribution;
                UpdateContribution.MonthForContribution = contributionDto.MonthForContribution;
                UpdateContribution.Amount = contributionDto.Amount;
                UpdateContribution.Type = contributionDto.Type;
                UpdateContribution.IsValid = false;
                UpdateContribution.IsProcessed = false;
                UpdateContribution.status = "Pending validation";
                UpdateContribution.Remarks = "";

                //  var contribution = _mapper.Map<Contribution>(contributionDto);
                await _unitOfWorkRepo.Contributions.UpdateContribution(UpdateContribution);
                var result = await _unitOfWorkRepo.CompleteAsync();
                if (result > 0)
                {
                    string ValidateContributionJobId = _backgroundJobClient.Enqueue(() => _ipcmsBackgroundService.ValidateContribution(contributionDto.ContributionId));
                    string UpdateIntrestJobId = _backgroundJobClient.ContinueJobWith(ValidateContributionJobId, () => _ipcmsBackgroundService.UpdateMemberInterest(contributionDto.ContributionId));

                    response.ResponseMessage = "Success";
                    response.ResponseCode = "00";
                    _logger.LogInformation($"Contribution details updated successfully for ContributionId: {contributionDto.ContributionId}");
                }
                else
                {
                    _logger.LogError($"Failed to update contribution details. see logs for details");
                    response.ResponseMessage = "Failed to update contribution details. see logs for details";
                    response.ResponseCode = "01";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error message {ex.Message}");
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
                    response.ResponseMessage = "failed to retrieve Member details";
                    response.ResponseCode = "01";
                }

                return response;


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiResponse<List<ContributionDto>>> GetMemberContributions(string MemberId)
        {
            try
            {
                var response = new ApiResponse<List<ContributionDto>>();

                var result = await _unitOfWorkRepo.Contributions.GetMemberContributions(MemberId);

                if (result != null)
                {
                    response.Data = _mapper.Map<List<ContributionDto>>(result);
                    response.ResponseMessage = "Success";
                    response.ResponseCode = "00";
                }
                else
                {
                    response.ResponseMessage = "Failed to retrieve Member details";
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
