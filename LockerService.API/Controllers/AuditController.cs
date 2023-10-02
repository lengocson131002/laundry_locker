using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Audits.Models;
using LockerService.Application.Audits.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/audits")]
[AuthorizeRoles(Role.Admin)]
[ApiKey]
public class AuditController: ApiControllerBase
{
    [HttpGet]
    public async Task<PaginationResponse<Audit, AuditResponse>> GetApiAudits([FromQuery] GetAuditsQuery request)
    {
        return await Mediator.Send(request);
    }
}