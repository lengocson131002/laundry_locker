using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.Projectables;
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

    [Projectable]
    public bool IsExpired => Equals(Status, TokenStatus.Invalid) || (ExpiredAt != null && ExpiredAt < DateTimeOffset.UtcNow);

    public DeviceType? DeviceType { get; set; }
    
    public Token()
    {
        Status = TokenStatus.Valid;
    }
}