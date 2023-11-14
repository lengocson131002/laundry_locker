using LockerService.API.Attributes;
using LockerService.Application.Features.Locations.Models;
using LockerService.Application.Features.Locations.Queries;

namespace LockerService.API.Controllers;

/// <summary>
/// API For address (Province, District, Wards)
/// </summary>
[ApiController]
[Route("/api/v1/addresses")]
[ApiKey]
public class AddressController : ApiControllerBase
{
    /// <summary>
    /// API Get all addresses. Get address by parent code
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<ListResponse<AddressResponse>>> GetAddresses([FromQuery] GetAddressesQuery request)
    {
        return await Mediator.Send(request);
    }
}