using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Stores.Commands;
using LockerService.Application.Features.Stores.Models;

namespace LockerService.Application.Features.Stores.Handlers;

public class AddStoreHandler : IRequestHandler<AddStoreCommand, StoreResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AddStoreHandler> _logger;
    private readonly IMapper _mapper;

    public AddStoreHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<AddStoreHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<StoreResponse> Handle(AddStoreCommand request, CancellationToken cancellationToken)
    {
        var store = _mapper.Map<Store>(request);

        if (store is null)
        {
            throw new ApiException(ResponseCode.MappingError);
        }

        //location
        var location = request.Location;

        var province = await _unitOfWork.AddressRepository.CheckProvince(location.ProvinceCode);
        if (province is null)
        {
            throw new ApiException(ResponseCode.AddressErrorProvinceNotFound);
        }

        var district = await _unitOfWork.AddressRepository.CheckDistrict(location.DistrictCode, province.Code!);
        if (district is null)
        {
            throw new ApiException(ResponseCode.AddressErrorDistrictNotFound);
        }

        var ward = await _unitOfWork.AddressRepository.CheckWardCode(location.WardCode, district.Code!);
        if (ward is null)
        {
            throw new ApiException(ResponseCode.AddressErrorWardNotFound);
        }

        store.Location = new Location()
        {
            Address = location.Address,
            Province = province,
            District = district,
            Ward = ward,
            Longitude = location.Longitude,
            Latitude = location.Latitude
        };

        await _unitOfWork.StoreRepository.AddAsync(store);

        // Save changes
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<StoreResponse>(store);
    }
}