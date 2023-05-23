using System.Security.Claims;

namespace LockerService.Application.Common.Services;

public interface ICurrentPrincipalService
{
    public string? CurrentPrincipal { get; }
    
    public long? CurrentSubjectId { get; }

    public ClaimsPrincipal GetCurrentPrincipalFromToken(string token);

}