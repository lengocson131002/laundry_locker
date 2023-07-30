namespace LockerService.Application.Common.Services.Notification.Data;

public class SmsNotificationData : NotificationData
{
    public string Phone { get; private set; }
    public string Message { get; private set; } 
    
    public SmsNotificationData(string phone, string message)
    {
        Phone = phone.ToVietnamesePhoneNumber();
        Message = message;
    }
}