using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LockerService.Domain.Enums;

namespace LockerService.Domain.Entities;

[Table("Token")]
public class Token : BaseAuditableEntity
{
    [Key] public int Id { get; set; }
    public string Content { get; set; }

    public int? accountId { get; set; }
    public Account? Account { get; set; }
    public DateTimeOffset? ExpirationTime { get; set; }
    public TokenStatus Status { get; set; } = TokenStatus.Valid;
    public TokenType Type { get; set; }
}