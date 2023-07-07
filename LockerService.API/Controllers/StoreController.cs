namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/stores")]
public class StoreController : ApiControllerBase
{
    [HttpGet("")]
    [AllowAnonymous]
    public async Task<ActionResult<PaginationResponse<Store, StoreResponse>>> GetAllStores(
        [FromQuery] GetAllStoresQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<StoreDetailResponse>> GetAllStores([FromRoute] int id)
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

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StoreResponse>> UpdateStore([FromRoute] int id,
        [FromBody] UpdateStoreCommand command)
    {
        command.StoreId = id;
        return await Mediator.Send(command);
    }

    [HttpPut("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StoreResponse>> ActivateStore([FromRoute] int id)
    {
        var command = new ActivateStoreCommand
        {
            StoreId = id
        };
        return await Mediator.Send(command);
    }

    [HttpPut("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StoreResponse>> DeactivateStore([FromRoute] int id)
    {
        var command = new DeactivateStoreCommand()
        {
            StoreId = id
        };
        return await Mediator.Send(command);
    }
}