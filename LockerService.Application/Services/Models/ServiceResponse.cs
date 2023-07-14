namespace LockerService.Application.Services.Models;

public class ServiceResponse
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public string Image { get; set; } = default!;

    public decimal Price { get; set; }

    public string? Description { get; set; }
    
    public string? Unit { get; set; }
    
    public ServiceStatus Status { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public long? CreatedBy { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public long? UpdatedBy { get; set; }

}