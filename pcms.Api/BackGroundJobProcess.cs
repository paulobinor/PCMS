using Hangfire;
using pcms.Application.Interfaces;

namespace pcms.Api
{
    public class BackGroundJobProcess
    {
        private readonly IPCMSBackgroundService _pCMSBackgroundService;
        private readonly ILogger<BackGroundJobProcess> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public BackGroundJobProcess(IPCMSBackgroundService pCMSBackgroundService, ILogger<BackGroundJobProcess> logger, IBackgroundJobClient backgroundJobClient)
        {
            _pCMSBackgroundService = pCMSBackgroundService;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
        }
        public void ProcessStartupTask()
        {
            try
            {
              //  _logger.LogInformation("This job is running successfully");
              //  Console.WriteLine("This job will execute");
               var jobId1 = _backgroundJobClient.Enqueue(() => _pCMSBackgroundService.ValidateMemberContributions());
                var jobId2 = _backgroundJobClient.ContinueJobWith(jobId1, () => _pCMSBackgroundService.UpdateBenefitEligibility());
                //_pCMSBackgroundService.ValidateMemberContributions().ContinueWith((n) => _pCMSBackgroundService.UpdateBenefitEligibility());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
