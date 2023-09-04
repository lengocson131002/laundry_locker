namespace LockerService.Application.Services.Models;

public class ServiceResponse : BaseAuditableEntityResponse
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public string Image { get; set; } = default!;

    public decimal Price { get; set; }

    public string? Description { get; set; }
    
    public string? Unit { get; set; }
    
    public ServiceStatus Status { get; set; }
    
}