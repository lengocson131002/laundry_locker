using LockerService.Domain.Enums;

namespace LockerService.API.Common;

public class AuthorizeRoles : AuthorizeAttribute
{
    public AuthorizeRoles(params Role[] roles)
    {
        Roles = string.Join(", ", roles);
    }
}