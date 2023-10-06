using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Locations.Models;
using LockerService.Application.Features.Locations.Queries;

namespace LockerService.Application.Features.Locations.Handlers;

public class GetAddressesHandler : IRequestHandler<GetAddressesQuery, ListResponse<AddressResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAddressesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ListResponse<AddressResponse>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.AddressRepository.GetAsync(
                predicate:address => address.ParentCode == request.ParentCode, 
                disableTracking: true
        );

        var response = query
            .ToList()
            .Select(address => _mapper.Map<AddressResponse>(address))
            .ToList();
        
        return new ListResponse<AddressResponse>(response);
    }
}