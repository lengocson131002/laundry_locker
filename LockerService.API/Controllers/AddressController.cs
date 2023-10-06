using LockerService.API.Attributes;
using LockerService.Application.Features.Locations.Models;
using LockerService.Application.Features.Locations.Queries;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/addresses")]
[ApiKey]
public class AddressController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ListResponse<AddressResponse>>> GetAddresses([FromQuery] GetAddressesQuery request)
    {
        return await Mediator.Send(request);
    }
}