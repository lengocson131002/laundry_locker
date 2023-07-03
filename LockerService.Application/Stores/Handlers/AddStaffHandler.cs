using LockerService.Application.Stores.Commands;

namespace LockerService.Application.Stores.Handlers;

public class AddStoreHandler : IRequestHandler<AddStoreRequest, StoreResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AddStaffHandler> _logger;
    private readonly IMapper _mapper;

    public AddStoreHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<AddStaffHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<StoreResponse> Handle(AddStoreRequest request, CancellationToken cancellationToken)
    {
        var store = _mapper.Map<Store>(request);

        if (store == null)
        {
            throw new ApiException(ResponseCode.MappingError);
        }

        //location
        var location = request.Location;
        var provinceQuery =
            await _unitOfWork.AddressRepository.GetAsync(p => p.Code != null && p.Code.Equals(location.ProvinceCode));
        var province = provinceQuery.FirstOrDefault();
        if (province == null)
        {
            throw new ApiException(ResponseCode.AddressErrorProvinceNotFound);
        }

        var districtQuery =
            await _unitOfWork.AddressRepository.GetAsync(d => d.Code != null && d.Code.Equals(location.DistrictCode));
        var district = districtQuery.FirstOrDefault();
        if (district == null || district.ParentCode != province.Code)
        {
            throw new ApiException(ResponseCode.AddressErrorDistrictNotFound);
        }

        var wardQuery =
            await _unitOfWork.AddressRepository.GetAsync(w => w.Code != null && w.Code.Equals(location.WardCode));
        var ward = wardQuery.FirstOrDefault();
        if (ward == null || ward.ParentCode != district.Code)
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