namespace LockerService.Domain.Entities.Settings;

public class OrderSettings
{
    public int InitTimeoutInMinutes { get; set; } 
    
    public int ReservationInitTimeoutInMinutes { get; set; }
    
    public decimal StoragePrice { get; set; }

    public decimal MaxTimeInHours { get; set; }
    
    public decimal ExtraFee { get; set; }
    
    public int MaxOrderCount { get; set; }

    public static OrderSettings Initialize()
    {
        return new OrderSettings();
    }
}