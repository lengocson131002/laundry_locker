using LockerService.Application.Features.Locations.Models;

namespace LockerService.Application.Features.Locations.Queries;

public class GetAddressesQuery : IRequest<ListResponse<AddressResponse>>
{
    public string? ParentCode { get; set; }
}