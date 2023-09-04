using LockerService.Application.Common.Enums;
using LockerService.Application.Services.Commands;
using LockerService.Application.Services.Models;
using LockerService.Application.Services.Queries;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/stores")]
public class StoreController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Store, StoreResponse>>> GetAllStores(
        [FromQuery] GetAllStoresQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.SortColumn))
        {
            request.SortColumn = "CreatedAt";
            request.SortDir = SortDirection.Desc;
        }

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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StoreResponse>> AddStore([FromBody] AddStoreCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StoreResponse>> UpdateStore(
        [FromRoute] long id,
        [FromBody] UpdateStoreCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }

    [HttpPut("{id:long}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StoreResponse>> UpdateStoreStatus(
        [FromRoute] long id,
        UpdateStoreStatusCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }

    // Services
    [HttpPost("{id:long}/services")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse>> AddService(
        [FromRoute] long id,
        [FromBody] AddServiceCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }

    [HttpGet("{id:long}/services")]
    public async Task<ActionResult<PaginationResponse<Service, ServiceResponse>>> GetAllServices(
        [FromRoute] long id,
        [FromQuery] GetAllServicesQuery query)
    {
        query.StoreId = id;

        if (string.IsNullOrWhiteSpace(query.SortColumn))
        {
            query.SortColumn = "UpdatedAt";
            query.SortDir = SortDirection.Desc;
        }

        return await Mediator.Send(query);
    }

    [HttpPut("{id:long}/services/{serviceId:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> UpdateService(
        [FromRoute] long id, 
        [FromRoute] long serviceId, 
        [FromBody] UpdateServiceCommand command)
    {
        command.StoreId = id;
        command.ServiceId = serviceId;
        
        await Mediator.Send(command);
        return new StatusResponse(true);
    }

    [HttpGet("{id:long}/services/{serviceId:long}")]
    public async Task<ActionResult<ServiceDetailResponse>> GetService([FromRoute] long id, [FromRoute] long serviceId)
    {
        var query = new GetServiceQuery(id, serviceId);
        return await Mediator.Send(query);
    }

    [HttpPut("{id:long}/services/{serviceId:long}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StatusResponse>> UpdateServiceStatus(
        [FromRoute] long id, 
        [FromRoute] long serviceId,
        [FromBody] UpdateServiceStatusCommand command)
    {
        command.StoreId = id;
        command.ServiceId = serviceId;
        
        await Mediator.Send(command);
        return new StatusResponse(true);
    }
}