using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Token")]
public class Token : BaseAuditableEntity
{
    [Key] 
    public long Id { get; set; }

    public long AccountId { get; set; }

    public Account Account { get; set; } = default!;
    
    public TokenType Type { get; set; }

    public TokenStatus Status { get; set; }

    public string Value { get; set; } = default!;
    
    public DateTimeOffset? ExpiredAt { get; set; }

    public bool IsExpired => ExpiredAt != null && ExpiredAt < DateTimeOffset.UtcNow;
}