namespace LockerService.Application.Common.Services.Notifications.Models;

public class FirebaseNotification
{
    [JsonPropertyName("message")]
    public FirebaseNotificationMessage Message { get; set; } = default!;
    
}

public class FirebaseNotificationMessage
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = default!;

    [JsonPropertyName("notification")]
    public FirebaseNotificationContent Notification { get; set; } = default!;

    [JsonPropertyName("data")] 
    public object? Data { get; set; } = default!;
    
    [JsonPropertyName("android")]
    public FirebaseAndroidOptions? Android { get; set; } = default!;

    [JsonPropertyName("apns")]
    public FirebaseIosOptions? Apns = default!;
}

public class FirebaseNotificationContent
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("body")]
    public string Body { get; set; } = default!;
    
}

public class FirebaseAndroidOptions
{
    [JsonPropertyName("ttl")]
    public string? Ttl { get; set; } = default!;

    [JsonPropertyName("notification")]
    public FirebaseAndroidNotification? Notification { get; set; } = default!; 
    
    public class FirebaseAndroidNotification
    {
        [JsonPropertyName("click_action")] 
        public string? ClickAction { get; set; } = default!;
    }
}

public class FirebaseIosOptions
{
    
}