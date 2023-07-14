using System.Linq.Dynamic.Core;

namespace LockerService.Application.Staffs.Handlers;

public class RevokeStaffHandler : IRequestHandler<RevokeStaffCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RevokeStaffHandler> _logger;
    private readonly IMapper _mapper;

    public RevokeStaffHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<RevokeStaffHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<StatusResponse> Handle(RevokeStaffCommand request, CancellationToken cancellationToken)
    {
        var storeQuery =
            await _unitOfWork.StoreRepository.GetAsync(s =>
                Equals(s.Id, request.StoreId));
        var store = storeQuery.FirstOrDefault();
        if (store == null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        var accountQuery =
            await _unitOfWork.AccountRepository.GetAsync(a => Equals(a.Id, request.Id)
            );

        var account = accountQuery.FirstOrDefault();
        if (account is null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }

        var accountLockerQuery =
            await _unitOfWork.AccountLockerRepository.GetAsync(
                al => Equals(al.StaffId, request.Id) && Equals(al.LockerId, request.LockerId));
        var accountLocker = accountLockerQuery.FirstOrDefault();
        if (accountLocker is null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        await _unitOfWork.AccountLockerRepository.DeleteAsync(accountLocker);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return new StatusResponse(true);
    }
}