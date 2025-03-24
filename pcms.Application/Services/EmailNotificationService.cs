using Microsoft.Extensions.Logging;
using pcms.Domain.Interfaces;

namespace pcms.Application.Services
{
    public class EmailNotificationService : INotificationService
    {
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(ILogger<EmailNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string recipient, string subject, string message)
        {
            // Simulate sending an email
            _logger.LogInformation($"Email sent to {recipient} - {subject}");
            await Task.CompletedTask;
        }
    }
}
