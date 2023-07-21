using LockerService.Domain.Events;
using Newtonsoft.Json;

namespace LockerService.Application.Lockers.Handlers;

public class UpdateLockerHandler :
    IRequestHandler<UpdateLockerCommand>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLockerHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }


    public async Task Handle(UpdateLockerCommand request, CancellationToken cancellationToken)
    {
        var lockerQuery = await _unitOfWork.LockerRepository.GetAsync(
            locker => locker.Id == request.LockerId,
            includes: new List<Expression<Func<Locker, object>>>
            {
                locker => locker.Location,
                locker => locker.Location.Province,
                locker => locker.Location.District,
                locker => locker.Location.Ward,
            });

        var locker = lockerQuery.FirstOrDefault();
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        locker.Name = request.Name ?? locker.Name;
        locker.Description = request.Description ?? locker.Description;
        locker.Image = request.Image ?? locker.Image;

        if (request.StoreId is not null)
        {
            var storeQuery = await _unitOfWork.StoreRepository.GetAsync(
                store => store.Id == request.StoreId);

            var store = storeQuery.FirstOrDefault();
            if (store is null)
            {
                throw new ApiException(ResponseCode.StoreErrorNotFound);
            }

            locker.StoreId = request.StoreId;
        }

        // Assign staff
        if (request.StaffIds?.Any() ?? false)
        {
            locker.StaffLockers.Clear();

            var slQuery = await _unitOfWork.StaffLockerRepository.GetAsync(
                predicate: sl => Equals(sl.LockerId, locker.Id));

            await slQuery.ForEachAsync(async sl => { await _unitOfWork.StaffLockerRepository.DeleteAsync(sl); },
                cancellationToken: cancellationToken);

            request.StaffIds.Distinct().ForEach(async staffId =>
            {
                var staffQuery = await _unitOfWork.AccountRepository.GetAsync(
                    predicate: staff => staff.Id == staffId
                                        && Equals(staff.StoreId, locker.StoreId));

                var staff = staffQuery.FirstOrDefault();
                if (staff is null) return;

                var staffLocker = new StaffLocker
                {
                    Staff = staff,
                    Locker = locker,
                };

                locker.StaffLockers.Add(staffLocker);
            });
        }

        if (request.Location != null)
        {
            var location = request.Location;
            var provinceQuery =
                await _unitOfWork.AddressRepository.GetAsync(
                    p => p.Code != null && p.Code.Equals(location.ProvinceCode));
            var province = await provinceQuery.FirstOrDefaultAsync();
            if (province == null) throw new ApiException(ResponseCode.AddressErrorProvinceNotFound);

            var districtQuery =
                await _unitOfWork.AddressRepository.GetAsync(
                    d => d.Code != null && d.Code.Equals(location.DistrictCode));
            var district = await districtQuery.FirstOrDefaultAsync();
            if (district == null || district.ParentCode != province.Code)
                throw new ApiException(ResponseCode.AddressErrorDistrictNotFound);

            var wardQuery =
                await _unitOfWork.AddressRepository.GetAsync(w => w.Code != null && w.Code.Equals(location.WardCode));
            var ward = await wardQuery.FirstOrDefaultAsync();
            if (ward == null || ward.ParentCode != district.Code)
                throw new ApiException(ResponseCode.AddressErrorWardNotFound);

            locker.Location.Address = location.Address;
            locker.Location.Province = province;
            locker.Location.District = district;
            locker.Location.Ward = ward;
            locker.Location.Longitude = location.Longitude;
            locker.Location.Latitude = location.Latitude;
        }

        await _unitOfWork.LockerRepository.UpdateAsync(locker);
        await _unitOfWork.LocationRepository.UpdateAsync(locker.Location);

        // Create timeline
        var lockerEvent = new LockerTimeline()
        {
            Locker = locker,
            Event = LockerEvent.UpdateInformation,
            Status = locker.Status,
            Data = JsonConvert.SerializeObject(locker)
        };
        await _unitOfWork.LockerTimelineRepository.AddAsync(lockerEvent);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
    }
}