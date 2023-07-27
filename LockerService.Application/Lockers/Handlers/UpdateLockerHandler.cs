using LockerService.Application.EventBus.RabbitMq;
using LockerService.Application.EventBus.RabbitMq.Events.Lockers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LockerService.Application.Lockers.Handlers;

public class UpdateLockerHandler : IRequestHandler<UpdateLockerCommand>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRabbitMqBus _rabbitMqBus;
    
    public UpdateLockerHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork, IRabbitMqBus rabbitMqBus)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _rabbitMqBus = rabbitMqBus;
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
                locker => locker.Location.Ward
            });

        var locker = lockerQuery.FirstOrDefault();
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        if (request.Name != null && !Equals(request.Name, locker.Name))
        {
            if (await _unitOfWork.LockerRepository.FindByName(request.Name) != null)
            {
                throw new ApiException(ResponseCode.LockerErrorExistedName);
            }

            locker.Name = request.Name;
        }

        if (request.Image != null)
        {
            locker.Image = request.Image;
        }

        if (request.Description != null)
        {
            locker.Description = request.Description;
        }

        if (request.StoreId != null && !Equals(request.StoreId, locker.StoreId))
        {
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                throw new ApiException(ResponseCode.StoreErrorNotFound);
            }
            locker.StoreId = request.StoreId.Value;
        }

        if (request.StaffIds != null)
        {
            var staffLockers = new List<StaffLocker>();
            foreach (var staffId in request.StaffIds)
            {
                var staff = await _unitOfWork.AccountRepository.GetStaffById(staffId);
                if (staff == null || !Equals(staff.StoreId, locker.StoreId))
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

            var currentStaffLockersQuery =
                await _unitOfWork.StaffLockerRepository.GetAsync(item => item.LockerId == locker.Id);

            await _unitOfWork.StaffLockerRepository.DeleteRange(currentStaffLockersQuery.ToList());
            await _unitOfWork.StaffLockerRepository.AddRange(staffLockers);
        }

        if (request.Location != null)
        {
            var location = request.Location;
            var provinceQuery =
                await _unitOfWork.AddressRepository.GetAsync(
                    p => p.Code != null && p.Code.Equals(location.ProvinceCode));
            var province = await provinceQuery.FirstOrDefaultAsync();
            if (province == null)
            {
                throw new ApiException(ResponseCode.AddressErrorProvinceNotFound);
            }

            var districtQuery =
                await _unitOfWork.AddressRepository.GetAsync(
                    d => d.Code != null && d.Code.Equals(location.DistrictCode));
            var district = await districtQuery.FirstOrDefaultAsync();
            if (district == null || district.ParentCode != province.Code)
            {
                throw new ApiException(ResponseCode.AddressErrorDistrictNotFound);
            }

            var wardQuery =
                await _unitOfWork.AddressRepository.GetAsync(w => w.Code != null && w.Code.Equals(location.WardCode));
            var ward = await wardQuery.FirstOrDefaultAsync();
            if (ward == null || ward.ParentCode != district.Code)
            {
                throw new ApiException(ResponseCode.AddressErrorWardNotFound);
            }

            locker.Location.Address = location.Address;
            locker.Location.Province = province;
            locker.Location.District = district;
            locker.Location.Ward = ward;
            locker.Location.Longitude = location.Longitude;
            locker.Location.Latitude = location.Latitude;
        }

        await _unitOfWork.LockerRepository.UpdateAsync(locker);
        await _unitOfWork.LocationRepository.UpdateAsync(locker.Location);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        
        await _rabbitMqBus.PublishAsync(new LockerUpdatedInfoEvent()
        {
            Id = locker.Id,
            Time = DateTimeOffset.UtcNow,
            Data = JsonSerializer.Serialize(locker)
        }, cancellationToken);
    }
}