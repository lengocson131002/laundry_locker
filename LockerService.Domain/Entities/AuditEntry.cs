using System.Text.Json;
using System.Text.Json.Serialization;
using LockerService.Domain.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LockerService.Domain.Entities;

public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        
    }

    public EntityEntry Entry { get; } = default!;
    
    public string TableName { get; set; } = default!;

    public Dictionary<string, object> KeyValues { get; } = new();

    public Dictionary<string, object> OldValues { get; } = new();

    public Dictionary<string, object> NewValues { get; } = new();
    
    public AuditType AuditType { get; set; }

    public List<string> ChangedColumns { get; set; } = new();

    public Audit ToAudit()
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(), },
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        
        var audit = new Audit();
        audit.TableName = TableName;
        audit.Type = AuditType.ToString();
        audit.PrimaryKey = JsonSerializer.Serialize(KeyValues, jsonOptions);
        audit.OldValues = OldValues.Any() ? JsonSerializer.Serialize(OldValues, jsonOptions) : null;
        audit.NewValues = NewValues.Any() ? JsonSerializer.Serialize(NewValues, jsonOptions) : null;
        audit.AffectedColumns = ChangedColumns.Any() ? JsonSerializer.Serialize(ChangedColumns) : null;
        audit.CreatedAt = DateTimeOffset.UtcNow;
        audit.UpdatedAt = DateTimeOffset.UtcNow;
        return audit;
    }
}