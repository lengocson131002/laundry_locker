using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Common.Enums;
using LockerService.Application.Features.Services.Commands;
using LockerService.Application.Features.Services.Models;
using LockerService.Application.Features.Services.Queries;
using LockerService.Application.Features.Stores.Commands;
using LockerService.Application.Features.Stores.Models;
using LockerService.Application.Features.Stores.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/stores")]
[ApiKey]
public class StoreController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Store, StoreResponse>>> GetAllStores(
        [FromQuery] GetAllStoresQuery request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<StoreDetailResponse>> GetStore([FromRoute] long id)
    {
        return await Mediator.Send(new GetStoreQuery
        {
            StoreId = id
        });
    }

    [HttpPost("")]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<StoreResponse>> AddStore([FromBody] AddStoreCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id:long}")]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<StoreResponse>> UpdateStore(
        [FromRoute] long id,
        [FromBody] UpdateStoreCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }

    [HttpPut("{id:long}/status")]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<StoreResponse>> UpdateStoreStatus(
        [FromRoute] long id,
        UpdateStoreStatusCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }


    ///<summary>
    /// Config service created by System Administrator for specific store, update price only
    /// </summary>
    [HttpPost("{id:long}/services")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> AddServiceToStore(
        [FromRoute] long id,
        [FromBody] ConfigStoreServiceCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Remove specific service from specific store, which configured from above
    /// </summary>

    [HttpDelete("{id:long}/services/{serviceId:long}")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> RemoveServiceFromStore(
        [FromRoute] long id,
        [FromRoute] long serviceId)
    {
        var command = new RemoveStoreServiceCommand(id, serviceId);
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Update specific service's price which configured from standard service for specific store
    /// </summary>
    
    [HttpPut("{id:long}/services/{serviceId:long}")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> UpdateServicePriceForStore(
        [FromRoute] long id,
        [FromRoute] long serviceId,
        [FromBody] UpdateStoreServiceCommand command)
    {
        command.StoreId = id;
        command.ServiceId = serviceId;
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Get specific store's service, don't use /api/v1/service/{id} before the price is configured for specific store
    /// </summary>
    
    [HttpGet("{id:long}/services/{serviceId:long}")]
    public async Task<ActionResult<ServiceDetailResponse>> GetStoreServiceDetail([FromRoute] long id, [FromRoute] long serviceId)
    {
        var query = new GetStoreServiceQuery(id, serviceId);
        return await Mediator.Send(query);
    }
}