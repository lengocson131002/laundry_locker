namespace LockerService.Application.Features.Auth.Models;

public class TokenResponse
{
    public long Id { get; set; }

    public long AccountId { get; set; }
    
    public TokenType Type { get; set; }

    public TokenStatus Status { get; set; }

    public string Value { get; set; } = default!;
    
    public DateTimeOffset? ExpiredAt { get; set; }

    public bool IsExpired { get; set; }

    public DeviceType? DeviceType { get; set; }
}