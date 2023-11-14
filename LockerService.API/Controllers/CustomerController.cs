using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Common.Enums;
using LockerService.Application.Features.Customers.Commands;
using LockerService.Application.Features.Customers.Models;
using LockerService.Application.Features.Customers.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

/// <summary>
/// CUSTOMER API
/// </summary>
[ApiController]
[Route("/api/v1/customers")]
[ApiKey]
public class CustomerController : ApiControllerBase
{
    /// <summary>
    /// get all customer
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PaginationResponse<Account, CustomerResponse>>> GetAllCustomers(
        [FromQuery] GetAllCustomersQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.SortColumn))
        {
            query.SortColumn = "CreatedAt";
            query.SortDir = SortDirection.Desc;
        }

        return await Mediator.Send(query);
    }

    /// <summary>
    /// Get a customer details
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:long}")]
    [Authorize]
    public async Task<ActionResult<CustomerDetailResponse>> GetCustomerDetail([FromRoute] long id)
    {
        var query = new GetCustomerDetailQuery(id);
        return await Mediator.Send(query);
    }
    
    /// <summary>
    /// Query a customer by phone number
    /// </summary>
    /// <param name="phone"></param>
    /// <returns></returns>
    [HttpGet("by-phone")]
    public async Task<ActionResult<CustomerDetailResponse>> GetCustomerDetail([FromQuery] string phone)
    {
        var query = new GetCustomerByPhoneQuery(phone.Trim());
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Update customer status
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{id:long}")]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<StatusResponse>> UpdateCustomerStatus([FromRoute] long id, [FromBody] UpdateCustomerCommand command)
    {
        command.Id = id;
        await Mediator.Send(command);
        return new StatusResponse(true);
    }
}