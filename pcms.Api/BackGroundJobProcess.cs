using Hangfire;
using pcms.Application.Interfaces;
using pcms.Domain.Interfaces;

namespace pcms.Api
{
    public class BackGroundJobProcess
    {
        private readonly IPCMSBackgroundService _pCMSBackgroundService;
        private readonly ILogger<BackGroundJobProcess> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IUnitOfWorkRepo _unitOfWorkRepo;
        public BackGroundJobProcess(IPCMSBackgroundService pCMSBackgroundService, ILogger<BackGroundJobProcess> logger, IBackgroundJobClient backgroundJobClient, IUnitOfWorkRepo unitOfWorkRepo)
        {
            _pCMSBackgroundService = pCMSBackgroundService;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _unitOfWorkRepo = unitOfWorkRepo;
        }
        public async Task ProcessStartupTask()
        {
            try
            {
              //  _logger.LogInformation("This job is running successfully");
              //  Console.WriteLine("This job will execute");
               var jobId1 = _backgroundJobClient.Enqueue(() => _pCMSBackgroundService.ValidateMemberContributions());
                var members = await _unitOfWorkRepo.Members.GetAllMembers();
                if (members != null)
                {
                    var jobId2 = _backgroundJobClient.ContinueJobWith(jobId1, () => _pCMSBackgroundService.UpdateBenefitEligibility(members));

                }
                //_pCMSBackgroundService.ValidateMemberContributions().ContinueWith((n) => _pCMSBackgroundService.UpdateBenefitEligibility());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
