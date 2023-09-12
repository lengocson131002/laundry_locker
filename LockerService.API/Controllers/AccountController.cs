using LockerService.API.Attributes;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/accounts")]
[ApiKey]
public class AccountController : ApiControllerBase
{
}