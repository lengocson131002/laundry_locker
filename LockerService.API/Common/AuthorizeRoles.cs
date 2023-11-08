using LockerService.Domain.Enums;

namespace LockerService.API.Common;

///
/// This attribute used for authorizing by roles
/// 
public class AuthorizeRoles : AuthorizeAttribute
{
    public AuthorizeRoles(params Role[] roles)
    {
        Roles = string.Join(", ", roles);
    }
}