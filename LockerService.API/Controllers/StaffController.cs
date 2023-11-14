using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Features.Staffs.Commands;
using LockerService.Application.Features.Staffs.Models;
using LockerService.Application.Features.Staffs.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

/// <summary>
/// STAFF API
/// </summary>
[ApiController]
[Route("/api/v1/staffs")]
[AuthorizeRoles(Role.Admin, Role.Manager)]
[ApiKey]
public class StaffController : ApiControllerBase
{
    /// <summary>
    /// Add new staff
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("")]
    public async Task<ActionResult<StaffDetailResponse>> AddStaff(AddStaffCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Get all staffs
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("")]
    public async Task<ActionResult<PaginationResponse<Account, StaffResponse>>> GetAllStaffs(
        [FromQuery] GetAllStaffsQuery request)
    {
        return await Mediator.Send(request);
    }

    /// <summary>
    /// Get a staff detail
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<StaffDetailResponse>> GetStaff([FromRoute] long id)
    {
        var query = new GetStaffQuery
        {
            Id = id,
        };
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Update a staff
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<StaffDetailResponse>> UpdateStaff(
        [FromRoute] long id,
        UpdateStaffCommand command)
    {
        command.Id = id;
        return await Mediator.Send(command);
    }

}