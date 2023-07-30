namespace LockerService.Application.Common.Services.Notifications;

public interface ISmsNotificationService : INotificationService
{
    public Task SendSmsAsync(string phoneNumber, string content);
}