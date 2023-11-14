using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Features.Audits.Models;
using LockerService.Application.Features.Audits.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

/// <summary>
/// AUDIT API
/// </summary>
[ApiController]
[Route("/api/v1/audits")]
[AuthorizeRoles(Role.Admin)]
[ApiKey]
public class AuditController: ApiControllerBase
{
    /// <summary>
    /// Get all audits
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<PaginationResponse<Audit, AuditResponse>> GetApiAudits([FromQuery] GetAuditsQuery request)
    {
        return await Mediator.Send(request);
    }
}