namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/stores")]
public class StoreController : ApiControllerBase
{
    [HttpGet("")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaginationResponse<Store, StoreResponse>>> GetAllStores(
        [FromQuery] GetAllStoresQuery request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StoreDetailResponse>> GetAllStores([FromRoute] long id)
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
    public async Task<ActionResult<StoreResponse>> UpdateStore([FromRoute] long id,
        [FromBody] UpdateStoreCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }

    [HttpPut("{id:long}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StoreResponse>> UpdateStoreStatus([FromRoute] long id,
        UpdateStoreStatusCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }
}