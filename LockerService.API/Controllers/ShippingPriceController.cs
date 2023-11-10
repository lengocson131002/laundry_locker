using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Features.ShippingPrices.Commands;
using LockerService.Application.Features.ShippingPrices.Models;
using LockerService.Application.Features.ShippingPrices.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/shipping-prices")]
[ApiKey]
public class ShippingPriceController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ListResponse<ShippingPriceResponse>>> GetAllShippingPrices()
    {
        var query = new GetAllShippingPricesQuery();
        return await Mediator.Send(query);
    }

    [HttpPost]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<ShippingPriceResponse>> AddShippingPrice([FromBody] AddShippingPriceCommand command)
    {
        return await Mediator.Send(command);
    }
    
    [HttpPut("{id:long}")]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<ShippingPriceResponse>> UpdateShippingPrice([FromRoute] long id, [FromBody] UpdateShippingPriceCommand command)
    {
        command.ShippingPriceId = id;
        return await Mediator.Send(command);
    }

    [HttpDelete("{id:long}")]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<ShippingPriceResponse>> RemoveShippingPrice([FromRoute] long id)
    {
        var command = new RemoveShippingPriceCommand(id);
        return await Mediator.Send(command);
    }

}