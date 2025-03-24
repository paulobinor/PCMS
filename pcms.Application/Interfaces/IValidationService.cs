namespace pcms.Application.Interfaces
{
    public interface IValidationService
    {
        Task<string> ValidateEmployer(string name);
    }
}