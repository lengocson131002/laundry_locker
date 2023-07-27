namespace LockerService.Domain.Entities.Settings;

public class OrderSettings : ISetting
{
    public int InitTimeoutInMinutes { get; set; } 
    
    public int ReservationInitTimeoutInMinutes { get; set; }
    
    public decimal StoragePrice { get; set; }

    public float MaxTimeInHours { get; set; }
    
    public decimal ExtraFee { get; set; }
    
    public int MaxActiveOrderCount { get; set; }

    public OrderSettings()
    {
        InitTimeoutInMinutes = 5;
        ReservationInitTimeoutInMinutes = 30;
        StoragePrice = 3000;
        MaxTimeInHours = 48;
        ExtraFee = 3000;
        MaxActiveOrderCount = 5;
    }
}