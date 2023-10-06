using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Staffs.Commands;

namespace LockerService.Application.Features.Staffs.Handlers;

public class ActivateStaffHandler : IRequestHandler<ActivateStaffCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<ActivateStaffHandler> _logger;
    private readonly IMapper _mapper;

    public ActivateStaffHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<ActivateStaffHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<StatusResponse> Handle(ActivateStaffCommand request, CancellationToken cancellationToken)
    {
        var storeQuery =
            await _unitOfWork.StoreRepository.GetAsync(s =>
                Equals(s.Id, request.StoreId));
        var store = storeQuery.FirstOrDefault();
        if (store == null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        var staff = await _unitOfWork.AccountRepository.GetStaffById(request.Id);

        if (staff is null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }

        if (staff.Status != AccountStatus.Inactive)
        {
            throw new ApiException(ResponseCode.StaffErrorInvalidStatus);
        }

        staff.Status = AccountStatus.Active;

        await _unitOfWork.AccountRepository.UpdateAsync(staff);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return new StatusResponse(true);
    }
}