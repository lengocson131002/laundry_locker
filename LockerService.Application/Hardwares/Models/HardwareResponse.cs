namespace LockerService.Application.Hardwares.Models;

public class HardwareResponse
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }
    
    public string? Brand { get; set; }

    public double? Price { get; set; }
    
    public string? Description { get; set; }
    
    public bool IsActive { get; set; }
}