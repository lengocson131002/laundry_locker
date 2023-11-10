using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Common.Enums;
using LockerService.Application.Features.Customers.Commands;
using LockerService.Application.Features.Customers.Models;
using LockerService.Application.Features.Customers.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/customers")]
[ApiKey]
public class CustomerController : ApiControllerBase
{
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

    [HttpGet("{id:long}")]
    [Authorize]
    public async Task<ActionResult<CustomerDetailResponse>> GetCustomerDetail([FromRoute] long id)
    {
        var query = new GetCustomerDetailQuery(id);
        return await Mediator.Send(query);
    }
    
    [HttpGet("by-phone")]
    public async Task<ActionResult<CustomerDetailResponse>> GetCustomerDetail([FromQuery] string phone)
    {
        var query = new GetCustomerByPhoneQuery(phone.Trim());
        return await Mediator.Send(query);
    }

    [HttpPut("{id:long}")]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<StatusResponse>> UpdateCustomerStatus([FromRoute] long id, [FromBody] UpdateCustomerCommand command)
    {
        command.Id = id;
        await Mediator.Send(command);
        return new StatusResponse(true);
    }
}