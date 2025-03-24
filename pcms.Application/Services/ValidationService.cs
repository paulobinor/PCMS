using pcms.Application.Interfaces;

namespace pcms.Infra
{
    public class ValidationService : IValidationService
    {
        public ValidationService() { }
        public async Task<string> ValidateEmployer(string name)
        {
            return "00";
        }
    }
}
