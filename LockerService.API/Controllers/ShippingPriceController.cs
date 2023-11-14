using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Features.ShippingPrices.Commands;
using LockerService.Application.Features.ShippingPrices.Models;
using LockerService.Application.Features.ShippingPrices.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

/// <summary>
/// SHIPPING PRICE API
/// </summary>
[ApiController]
[Route("/api/v1/shipping-prices")]
[ApiKey]
public class ShippingPriceController : ApiControllerBase
{
    /// <summary>
    /// Get all shipping prices
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ListResponse<ShippingPriceResponse>>> GetAllShippingPrices()
    {
        var query = new GetAllShippingPricesQuery();
        return await Mediator.Send(query);
    }

    /// <summary>
    /// Add new shipping price
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<ShippingPriceResponse>> AddShippingPrice([FromBody] AddShippingPriceCommand command)
    {
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Update a shipping price
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{id:long}")]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<ShippingPriceResponse>> UpdateShippingPrice([FromRoute] long id, [FromBody] UpdateShippingPriceCommand command)
    {
        command.ShippingPriceId = id;
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Remove s shipping price
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:long}")]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<ShippingPriceResponse>> RemoveShippingPrice([FromRoute] long id)
    {
        var command = new RemoveShippingPriceCommand(id);
        return await Mediator.Send(command);
    }

}