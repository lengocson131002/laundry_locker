using LockerService.Application.Common.Enums;
using LockerService.Application.Customers.Models;
using LockerService.Application.Customers.Queries;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/customers")]
public class CustomerController : ApiControllerBase
{
    [HttpGet]
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
    public async Task<ActionResult<CustomerDetailResponse>> GetCustomerDetail([FromRoute] long id)
    {
        var query = new GetCustomerDetailQuery(id);
        return await Mediator.Send(query);
    }
}