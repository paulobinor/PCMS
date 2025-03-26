using AutoMapper.Execution;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using pcms.Application.Dto;
using pcms.Application.Interfaces;
using pcms.Domain.Entities;
using pcms.Domain.Enums;
using pcms.Domain.Interfaces;
using System.Collections.Generic;

namespace pcms.Application.Services
{
    public class PCMSBackgroundService : IPCMSBackgroundService
    {
        private readonly IMemberContributionService _memberContributionService;
        private readonly ILogger<PCMSBackgroundService> _logger;
        private readonly IUnitOfWorkRepo _unitOfWorkRepo;
        private readonly ICacheService _cacheService;
        public PCMSBackgroundService(IMemberContributionService memberContributionService, IUnitOfWorkRepo unitOfWorkRepo, ILogger<PCMSBackgroundService> logger, ICacheService cacheService)
        {
            _memberContributionService = memberContributionService;
            _unitOfWorkRepo = unitOfWorkRepo;
            _logger = logger;
            _cacheService = cacheService;
        }
        public async Task UpdateBenefitEligibility()
        {
            _logger.LogInformation("About to compute benefit elegibility");
            var members = (await _unitOfWorkRepo.Members.GetAllMembers()).Where(m=>m.IsActive);

            _logger.LogInformation($"About to Process {members.Count()} records");
            int legible = 0;
            int Notlegible = 0;
            foreach (var member in members)
            {
                var ContributionList = (await _unitOfWorkRepo.Contributions.GetMemberContributions(member.MemberId)).Where(v => v.IsValid).OrderBy(m => m.EntryNumber);
                if (ContributionList == null)
                {
                    _logger.LogInformation($" no contributions found for {member.Name}");
                }
                else
                {
                    var lastContributionDate = ContributionList.Max(x => x.ContributionDate);
                    if (lastContributionDate != null)
                    {
                        _logger.LogInformation($"Last contribution date for {member.Name} is {lastContributionDate}");
                        if ((lastContributionDate - member.RegistrationDate).TotalDays >= 180)
                        {
                            _logger.LogInformation($"Last contribution date for {member.Name} is greater than registered date by {(lastContributionDate - member.RegistrationDate).TotalDays}");
                            member.IsEligibleForBenefit = true;
                            _unitOfWorkRepo.Members.UpdateMember(member);
                            _cacheService.SetData(member.MemberId, JsonConvert.SerializeObject(member), 3600);
                            legible++;
                        }
                        else
                        {
                            _logger.LogInformation($"{member.Name} does not meet the minimum requirement for eligibility");
                            Notlegible++;
                        }
                    }
                    else
                    {
                    }
                    _logger.LogInformation($"{member.Name} eligibility processed successfully processed");
                }
            }
            await _unitOfWorkRepo.CompleteAsync();
            _logger.LogInformation($"{members.Count()} records successfully processed with {legible} legible for benefits, while {Notlegible} not legible for benefits");
        }

        public async Task<ApiResponse<string>> ValidateContribution(string contributionId)
        {
            _logger.LogError($"About to validate contribution. ContributionId: {contributionId}");
            var response = new ApiResponse<string>();

            if (contributionId == null)
            {
                _logger.LogError("Invalid contributionId specified or null value provided");
                response.ResponseMessage = "Invalid contributionId specified or null value provided";
                response.ResponseCode = "01";
                return response;
            }
            var contribution = await _unitOfWorkRepo.Contributions.GetContribution(contributionId);
            bool IsValidTransaction = true;
            var member = await _unitOfWorkRepo.Members.GetMember(contribution.MemberId);
            int currentMonth = DateTime.Now.Month; // Get the current month

            var duplicate = member.Contributions
                    .Where(c => c.YearForContribution == contribution.YearForContribution && c.MonthForContribution == contribution.MonthForContribution && c.Type == ContributionType.Monthly);
            if (duplicate.Count() > 1)
            {
                _logger.LogError($"Duplicate contribution for ContributionId: {contributionId}");
                contribution.IsValid = false;
                contribution.Remarks = "Duplicate contribution";
                contribution.status = "Validation failed";
                IsValidTransaction = false;
                response.ResponseMessage = "Duplicate contribution";
                response.ResponseCode = "01";
            }

            if (IsValidTransaction)
            {
                if (contribution.Amount <= 0)
                {
                    _logger.LogError($"invalid amount provided for contributionId:{contributionId}");
                    IsValidTransaction = false;
                    contribution.IsValid = false;
                    contribution.Remarks = "Invalid amount of 0";
                    response.ResponseMessage = "Invalid amount of 0";
                    response.ResponseCode = "01";
                }
                else
                {
                    _logger.LogInformation($"validation successful for ContributionId: {contributionId}");
                    contribution.Remarks = "Approved";
                    contribution.status = "Validation Successful";
                    response.ResponseMessage = "Approved";
                    response.ResponseCode = "00";
                }
            }

            await _unitOfWorkRepo.CompleteAsync();
            _cacheService.SetData(member.MemberId, JsonConvert.SerializeObject(member), 3600);
            return response;
        }


        public async Task ValidateMemberContributions()
        {
            _logger.LogInformation("About to validate member contributions");
            var list = await _unitOfWorkRepo.Members.GetAllMembers();
            int currentMonth = DateTime.Now.Month;
            _logger.LogInformation($"Will process {list.Count} member records");
            foreach (var member in list)
            {
                _logger.LogInformation($"Now processing records for {member.Name}");
                var duplicates = member.Contributions
                     .Where(c => c.YearForContribution == DateTime.Now.Year)
                     .GroupBy(c => c.MonthForContribution)
                     .Where(g => g.Count() > 1)
                     .Select(g => g.Key) 
                     .ToList();
                if (duplicates != null)
                {
                    _logger.LogInformation($"There are currently {duplicates.Count} duplicate contributions for {member.Name}");
                }

                HashSet<int> contributedMonths =
                    new HashSet<int>(member.Contributions
                    .Where(c => c.YearForContribution == DateTime.Now.Year && c.Type == ContributionType.Monthly)
                    .Select(c => c.MonthForContribution));

                List<int> missingMonths = new List<int>();


                _logger.LogInformation($"Now checking for missing monthly contributions for {member.Name}");
                for (int month = 1; month <= currentMonth; month++)
                {
                    if (!contributedMonths.Contains(month))
                    {
                        _logger.LogError($"Contribution for month {month} of year {DateTime.Now.Year} is missing for {member.Name}");
                        missingMonths.Add(month);
                    }
                }

            }
            await _unitOfWorkRepo.CompleteAsync();
        }
        public async Task CalculateContributionInterest(List<MemberDto> members)
        {
            _logger.LogInformation($"Received request to calculate member interest");
            try
            {
                //var members = (await _unitOfWorkRepo.Members.GetAllMembers()).Where(m=>m.IsActive).ToList();

                //  int ProcessedCount = 0;
                //  int UnProcessed = 0;
                _logger.LogInformation($"Will process {members.Count} member records");
                foreach (var member in members)
                {
                    _logger.LogInformation($"Now processing {member.Name} interest");
                    var ContributionList = (await _unitOfWorkRepo.Contributions.GetMemberContributions(member.MemberId)).Where(v => v.IsValid).OrderBy(m => m.EntryNumber);

                    if (ContributionList != null)
                    {
                        decimal total = 0; // ContributionList.Sum(m => m.Amount);
                        foreach (var contribution in ContributionList)
                        {
                            total += contribution.Amount;
                            contribution.CumulativeContribution = total;
                            contribution.CumulativeIntrestAmount = (contribution.CumulativeContribution * 10) / 100;
                            contribution.TotalCumulative = contribution.CumulativeContribution + contribution.CumulativeIntrestAmount;
                            contribution.IsProcessed = true;
                            await _unitOfWorkRepo.Contributions.UpdateContribution(contribution);
                            _logger.LogInformation($"ContributionId: {contribution.ContributionId} for {member.Name} processed successfully");
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"No contribution records for {member.Name} or records not found");
                    }
                }
                await _unitOfWorkRepo.CompleteAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task CalculateContributionInterest(string memberId)
        {
            _logger.LogInformation($"Received request to calculate member interest");
            try
            {
                //var members = (await _unitOfWorkRepo.Members.GetAllMembers()).Where(m=>m.IsActive).ToList();

                //  int ProcessedCount = 0;
                //  int UnProcessed = 0;
                var ContributionList = (await _unitOfWorkRepo.Contributions.GetMemberContributions(memberId)).Where(v => v.IsValid).OrderBy(m => m.EntryNumber);

                if (ContributionList != null)
                {
                    decimal total = 0; // ContributionList.Sum(m => m.Amount);
                    foreach (var contribution in ContributionList)
                    {
                        total += contribution.Amount;
                        contribution.CumulativeContribution = total;
                        contribution.CumulativeIntrestAmount = (contribution.CumulativeContribution * 10) / 100;
                        contribution.TotalCumulative = contribution.CumulativeContribution + contribution.CumulativeIntrestAmount;
                        contribution.IsProcessed = true;
                        await _unitOfWorkRepo.Contributions.UpdateContribution(contribution);
                        _logger.LogInformation($"ContributionId: {contribution.ContributionId} for memberId : {memberId} processed successfully");
                    }
                }
                else
                {
                    _logger.LogInformation($"No contribution records for memberId: {memberId} or records not found");
                }
                await _unitOfWorkRepo.CompleteAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
