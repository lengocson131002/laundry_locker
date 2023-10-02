namespace LockerService.Application.Audits.Models;

public class AuditResponse : BaseAuditableEntityResponse
{
    public long Id { get; set; }
    
    public string Type { get; set; } = default!;
    
    public string TableName { get; set; } = default!;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; } = default!;

    public string? AffectedColumns { get; set; } = default!;

    public string PrimaryKey { get; set; } = default!;
}