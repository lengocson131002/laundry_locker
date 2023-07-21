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

        if (locker == null)
        {
            throw new ApiException(ResponseCode.MappingError);
        }

        // Check store
        var storeQuery =
            await _unitOfWork.StoreRepository.GetAsync(s =>
                Equals(s.Id, request.StoreId));
        var store = storeQuery.FirstOrDefault();
        if (store == null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
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


        locker.Code = LockerCodeUtils.GenerateLockerCode();

        // Assign staff
        request.StaffIds.Distinct().ForEach(async staffId =>
        {
            var staffQuery = await _unitOfWork.AccountRepository.GetAsync(
                predicate: staff => staff.Id == staffId
                                    && Equals(staff.Store, store));

            var staff = staffQuery.FirstOrDefault();
            if (staff is null) return;

            var staffLocker = new StaffLocker
            {
                Staff = staff,
                Locker = locker,
            };

            locker.StaffLockers.Add(staffLocker);
        });


        await _unitOfWork.LockerRepository.AddAsync(locker);

        // Save changes
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LockerResponse>(locker);
    }
}