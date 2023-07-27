namespace LockerService.Domain.Entities.Settings;

public class OrderSettings : ISetting
{
    public int InitTimeoutInMinutes { get; set; } 
    
    public int ReservationInitTimeoutInMinutes { get; set; }
    
    public decimal StoragePrice { get; set; }

    public int MaxTimeInHours { get; set; }
    
    public decimal ExtraFee { get; set; }
    
    public int MaxOrderCount { get; set; }

    public OrderSettings()
    {
        
    }
}