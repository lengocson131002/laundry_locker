namespace LockerService.Application.Features.Locations.Models;

public class LocationResponse
{
    public long Id { get; set; }
    
    public string? Address { get; set; }
    
    public double? Longitude { get; set; }
    
    public double? Latitude { get; set; }
    
    public string? Description { get; set; }

    public AddressResponse? Province { get; set; }
    
    public AddressResponse? District { get; set; }

    public AddressResponse? Ward { get; set; }
}