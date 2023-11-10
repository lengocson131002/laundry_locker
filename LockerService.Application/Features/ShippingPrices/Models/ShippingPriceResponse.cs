namespace LockerService.Application.Features.ShippingPrices.Models;

public class ShippingPriceResponse 
{
    public long Id { get; set; }
    
    public decimal Price { get; set; }
    
    public double FromDistance { get; set; } // In KM
    
}