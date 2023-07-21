namespace LockerService.Application.Lockers.Models;

public class LockerResponse
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;

    public string? Image { get; set; }

    public LockerStatus Status { get; set; }

    public string? Description { get; set; }

    public LocationResponse? Location { get; set; }
}