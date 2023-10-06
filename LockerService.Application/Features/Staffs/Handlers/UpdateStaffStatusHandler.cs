using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Staffs.Commands;
using LockerService.Application.Features.Staffs.Models;

namespace LockerService.Application.Features.Staffs.Handlers;

public class UpdateStaffStatusHandler : IRequestHandler<UpdateStaffStatusCommand, StaffDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UpdateStaffStatusHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateStaffStatusHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<UpdateStaffStatusHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<StaffDetailResponse> Handle(UpdateStaffStatusCommand request, CancellationToken cancellationToken)
    {
        var staff = await _unitOfWork.AccountRepository.GetByIdAsync(request.Id);
        if (staff is null || !staff.IsStaff)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }

        if (Equals(staff.Status, request.Status) 
            || Equals(staff.Status, AccountStatus.Verifying) 
            || Equals(request.Status, AccountStatus.Verifying))
        {
            throw new ApiException(ResponseCode.StaffErrorInvalidStatus);
        }

        staff.Status = request.Status;

        await _unitOfWork.AccountRepository.UpdateAsync(staff);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StaffDetailResponse>(staff);
    }
}