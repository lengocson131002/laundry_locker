using LockerService.API.Attributes;
using LockerService.API.Common;
using LockerService.Application.Common.Enums;
using LockerService.Application.Features.Services.Models;
using LockerService.Application.Features.Services.Queries;
using LockerService.Application.Features.Stores.Commands;
using LockerService.Application.Features.Stores.Models;
using LockerService.Application.Features.Stores.Queries;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

/// <summary>
/// Store API
/// </summary>
[ApiController]
[Route("/api/v1/stores")]
[ApiKey]
public class StoreController : ApiControllerBase
{
    /// <summary>
    /// Get all stores
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Store, StoreResponse>>> GetAllStores(
        [FromQuery] GetAllStoresQuery request)
    {
        return await Mediator.Send(request);
    }

    /// <summary>
    /// Get a store detail
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<StoreDetailResponse>> GetStore([FromRoute] long id)
    {
        return await Mediator.Send(new GetStoreQuery
        {
            StoreId = id
        });
    }

    /// <summary>
    /// Add a new store
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("")]
    [AuthorizeRoles(Role.Admin)]
    public async Task<ActionResult<StoreResponse>> AddStore([FromBody] AddStoreCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Update a store
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{id:long}")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StoreResponse>> UpdateStore(
        [FromRoute] long id,
        [FromBody] UpdateStoreCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Get all store's services
    /// </summary>
    /// <param name="id"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("{id:long}/services")]
    public async Task<ActionResult<PaginationResponse<Service, ServiceResponse>>> GetAllStoreServices([FromRoute] long id, [FromQuery] GetAllServicesQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.SortColumn))
        {
            query.SortColumn = "UpdatedAt";
            query.SortDir = SortDirection.Desc;
        }

        query.StoreId = id;
        return await Mediator.Send(query);
    }

    ///<summary>
    /// Add a new store's service
    /// </summary>
    [HttpPost("{id:long}/services")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<ServiceResponse>> AddStoreService(
        [FromRoute] long id,
        [FromBody] AddStoreServiceCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }
    
    ///<summary>
    /// Config a global service for specific store, update price only
    /// </summary>
    [HttpPost("{id:long}/services/config")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> AddStoreService(
        [FromRoute] long id,
        [FromBody] ConfigStoreServiceCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }

    
    /// <summary>
    /// Remove a service from a store
    /// </summary>
    /// <param name="id"></param>
    /// <param name="serviceId"></param>
    /// <returns></returns>
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
    /// Update store service. If service is global => update price only, if service was created by store, allow to update all information
    /// </summary>
    [HttpPut("{id:long}/services/{serviceId:long}")]
    [AuthorizeRoles(Role.Admin, Role.Manager)]
    public async Task<ActionResult<StatusResponse>> UpdateStoreService(
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