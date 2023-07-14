namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/addresses")]
public class AddressController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ListResponse<AddressResponse>>> GetAddresses([FromQuery] GetAddressesQuery request)
    {
        return await Mediator.Send(request);
    }
}