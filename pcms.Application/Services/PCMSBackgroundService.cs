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
            var list = (await _unitOfWorkRepo.Members.GetAllMembers()).Where(m=>m.IsActive);

            _logger.LogInformation($"About to Process {list.Count()} records");
            int legible = 0;
            int Notlegible = 0;
            foreach (var member in list)
            {
                var lastContributionDate = member.Contributions.Max(x => x.ContributionDate);
                if (lastContributionDate != null)
                {
                    if ((lastContributionDate.Month - member.RegistrationDate.Month) >= 6)
                    {
                        legible++;
                        member.IsEligibleForBenefit = true;
                        _unitOfWorkRepo.Members.UpdateMember(member);
                    }
                    Notlegible++;
                }
                _cacheService.SetData(member.MemberId, JsonConvert.SerializeObject(member), 36000);
            }
            await _unitOfWorkRepo.CompleteAsync();
            _logger.LogInformation($"{list.Count()} records successfully processed with {legible} legible for benefits, while {Notlegible} not legible for benefits");
        }

        public async Task<ApiResponse<string>> ValidateContribution(string contributionId)
        {
            var response = new ApiResponse<string>();
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
        public async Task ValidateLastMemberContribution(string memberId)
        {
            _logger.LogError($"Received request to validate member contribution with memberId: {memberId}");
            var lastContribution = (await _unitOfWorkRepo.Contributions.GetMemberContributions(memberId)).OrderByDescending(m => m.EntryNumber).FirstOrDefault();

            var contribution = await _unitOfWorkRepo.Contributions.GetContribution(lastContribution.ContributionId);
            bool IsValidTransaction = true;
            var member = await _unitOfWorkRepo.Members.GetMember(contribution.MemberId);
            int currentMonth = DateTime.Now.Month; // Get the current month

            var duplicate = member.Contributions
                    .Where(c => c.YearForContribution == contribution.YearForContribution && c.MonthForContribution == contribution.MonthForContribution && c.Type == ContributionType.Monthly);
            if (duplicate.Count() > 1)
            {
                _logger.LogError($"Duplicate contribution for ContributionId: {lastContribution.ContributionId}");
                contribution.IsValid = false;
                contribution.Remarks = "Duplicate contribution";
                IsValidTransaction = false;
            }

            if (IsValidTransaction)
            {
                if (contribution.Amount <= 0)
                {
                    _logger.LogError($"invalid amount provided for contributionId:{lastContribution.ContributionId}");
                    IsValidTransaction = false;
                    contribution.IsValid = false;
                    contribution.Remarks = "Invalid amount of 0";
                }
                else
                {
                    _logger.LogInformation($"validation successful for ContributionId: {lastContribution.ContributionId}");
                    contribution.Remarks = "Approved";
                }
            }

            await _unitOfWorkRepo.CompleteAsync();
            _cacheService.SetData(member.MemberId, JsonConvert.SerializeObject(member), 3600);
        }
        public async Task ValidateMemberContributions()
        {
            var list = await _unitOfWorkRepo.Members.GetAllMembers();
            int currentMonth = DateTime.Now.Month;
            foreach (var member in list)
            {
                var duplicates = member.Contributions
                     .Where(c => c.YearForContribution == DateTime.Now.Year)
                     .GroupBy(c => c.MonthForContribution)
                     .Where(g => g.Count() > 1)
                     .Select(g => g.Key) 
                     .ToList();


                HashSet<int> contributedMonths =
                    new HashSet<int>(member.Contributions
                    .Where(c => c.YearForContribution == DateTime.Now.Year && c.Type == ContributionType.Monthly)
                    .Select(c => c.MonthForContribution));

                List<int> missingMonths = new List<int>();


                for (int month = 1; month <= currentMonth; month++)
                {
                    if (!contributedMonths.Contains(month))
                    {
                        missingMonths.Add(month);
                    }
                }

            }
            await _unitOfWorkRepo.CompleteAsync();
        }

        public async Task UpdateMemberInterest(string memberId)
        {
            _logger.LogInformation($"Received request to update member interest with memberId: {memberId}");
            try
            {
                var LastContribution = (await _unitOfWorkRepo.Contributions.GetMemberContributions(memberId)).OrderByDescending(m => m.EntryNumber).FirstOrDefault();
                if (LastContribution != null)
                {
                    if (LastContribution.IsValid)
                    {
                        var total = await _memberContributionService.GetTotalContributionsAsync(memberId);
                        LastContribution.CumulativeContribution = total.Data + LastContribution.Amount;
                        LastContribution.CumulativeIntrestAmount = (LastContribution.CumulativeContribution * 10) / 100;
                        LastContribution.TotalCumulative = LastContribution.CumulativeContribution + LastContribution.CumulativeIntrestAmount;
                        LastContribution.status = "Interest Processed";
                        LastContribution.IsProcessed = true;
                        await _unitOfWorkRepo.Contributions.UpdateContribution(LastContribution);
                        await _unitOfWorkRepo.CompleteAsync();
                        _logger.LogError($"Interest with memberId: {memberId} updated successfully");
                    }
                }
            }
            catch (Exception)
            {
                _logger.LogError($"We encountered an error while updating member interest for MemberId: {memberId}.");
                throw;
            }
        }


        public async Task UpdateAllMemberInterest()
        {
            _logger.LogInformation($"Received request to update all member interest");
            try
            {
                var members = (await _unitOfWorkRepo.Members.GetAllMembers()).Where(m=>m.IsActive).ToList();

                int ProcessedCount = 0;
                int UnProcessed = 0;
                _logger.LogInformation($"Will process {members.Count} member records");
                foreach (var member in members) 
                {

                    _logger.LogInformation($"Now processing {member.Name} record");
                    var LastContribution = (await _unitOfWorkRepo.Contributions.GetMemberContributions(member.MemberId)).Where(v=>v.IsValid).OrderByDescending(m => m.EntryNumber).FirstOrDefault();
                    if (LastContribution.IsValid && !LastContribution.IsProcessed)
                    {
                        var total = await _memberContributionService.GetTotalContributionsAsync(member.MemberId);
                        LastContribution.CumulativeContribution = total.Data + LastContribution.Amount;
                        LastContribution.CumulativeIntrestAmount = (LastContribution.CumulativeContribution * 10) / 100;
                        LastContribution.TotalCumulative = LastContribution.CumulativeContribution + LastContribution.CumulativeIntrestAmount;
                        LastContribution.IsProcessed = true;
                        await _unitOfWorkRepo.Contributions.UpdateContribution(LastContribution);
                        ProcessedCount++;
                        _logger.LogInformation($"ContributionId: {LastContribution.ContributionId} for {member.Name} processed successfully");
                    }
                    else
                    {
                        UnProcessed++;
                        _logger.LogInformation($"ContributionId: {LastContribution.ContributionId} for {member.Name} not valid for processing. Reason: {LastContribution.Remarks}");
                    }
                }
                await _unitOfWorkRepo.CompleteAsync();
                _logger.LogInformation($"{members.Count()} records successfully processed with: {ProcessedCount} legible for benefits, while {UnProcessed} not legible for benefits");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
