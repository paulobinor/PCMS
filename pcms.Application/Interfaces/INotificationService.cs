namespace pcms.Domain.Interfaces
{
    public interface INotificationService
    {
        Task SendEmailAsync(string recipient, string subject, string message);
    }

}
