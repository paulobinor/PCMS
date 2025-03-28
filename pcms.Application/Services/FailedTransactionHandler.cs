﻿using Microsoft.Extensions.Logging;
using pcms.Application.Interfaces;
using pcms.Domain.Interfaces;

namespace pcms.Application.Services
{
    public class FailedTransactionHandler : IFailedTransactionHandler
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<FailedTransactionHandler> _logger;

        public FailedTransactionHandler(INotificationService notificationService, ILogger<FailedTransactionHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleFailedTransactionAsync(string jobId, string errorMessage)
        {
            _logger.LogError($"Hangfire Job Failed: {jobId}. Error: {errorMessage}");

            string adminEmail = "admin@nlpc.com";
            string subject = "Pension System - Failed Transaction Alert";
            string message = $"Transaction with Job ID: {jobId} failed. Error: {errorMessage}";

            await _notificationService.SendEmailAsync(adminEmail, subject, message);
        }
    }

}
