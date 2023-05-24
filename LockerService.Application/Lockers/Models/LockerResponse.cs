namespace LockerService.Application.Lockers.Models;

public class LockerResponse
{
    public int Id { get; set; }

    public string? Name { get; set; }
    
    public string? Code { get; set; }

    public LockerStatus Status { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int? Depth { get; set; }

    public string? Description { get; set; }
    
    public int RowCount { get; set; }
    
    public int ColumnCount { get; set; }
    
    public int BoxCount { get; set; }
    
    public LocationResponse? Location { get; set; }
    
    public string? Provider { get; set; }

    public string? MacAddress { get; set; }
}