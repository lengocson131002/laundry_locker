namespace LockerService.Application.Locations.Queries;

public class GetAddressesQuery : IRequest<ListResponse<AddressResponse>>
{
    public string? ParentCode { get; set; }
}