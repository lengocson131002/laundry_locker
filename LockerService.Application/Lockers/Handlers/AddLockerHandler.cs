using LockerService.Application.Common.Utils;

namespace LockerService.Application.Lockers.Handlers;

public class AddLockerHandler : IRequestHandler<AddLockerCommand, LockerResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AddLockerHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<LockerResponse> Handle(AddLockerCommand request,
        CancellationToken cancellationToken)
    {
        var locker = _mapper.Map<Locker>(request);
        locker.Code = LockerCodeUtils.GenerateLockerCode();

        if (locker == null)
        {
            throw new ApiException(ResponseCode.MappingError);
        }

        // Check store
        var store = await _unitOfWork.StoreRepository.GetByIdAsync(request.StoreId);
        if (store == null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        if (!store.IsActive)
        {
            throw new ApiException(ResponseCode.StoreErrorInvalidStatus);
        }

        // Check name
        if (await _unitOfWork.LockerRepository.FindByName(locker.Name) != null)
        {
            throw new ApiException(ResponseCode.LockerErrorExistedName);
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

        locker.Location = new Location()
        {
            Address = location.Address,
            Province = province,
            District = district,
            Ward = ward,
            Longitude = location.Longitude,
            Latitude = location.Latitude
        };

        var staffLockers = new List<StaffLocker>();
        foreach (var staffId in request.StaffIds)
        {
            var staff = await _unitOfWork.AccountRepository.GetByIdAsync(staffId);
            if (staff == null || !Equals(staff.Role, Role.Staff) || !Equals(staff.StoreId, store.Id))
            {
                throw new ApiException(ResponseCode.StaffErrorNotFound);
            }

            if (!staff.IsActive)
            {
                throw new ApiException(ResponseCode.StaffErrorInvalidStatus);
            }
            staffLockers.Add(new StaffLocker()
            {
                Locker = locker,
                Staff = staff
            });
        }
        
        await _unitOfWork.LockerRepository.AddAsync(locker);
        await _unitOfWork.StaffLockerRepository.AddRange(staffLockers);
        
        // Save changes
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LockerResponse>(locker);
    }
}