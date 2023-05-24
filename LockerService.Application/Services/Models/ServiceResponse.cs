namespace LockerService.Application.Services.Models;

public class ServiceResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string Image { get; set; } = default!;

    public double? Fee { get; set; }

    public FeeType FeeType { get; set; }

    public string? Description { get; set; }
    
    public string? Unit { get; set; }
    
    public bool IsActive { get; set; }
}