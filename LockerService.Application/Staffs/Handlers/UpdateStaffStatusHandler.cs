namespace LockerService.Application.Staffs.Handlers;

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
        var staff = await _unitOfWork.AccountRepository.GetStaffById(request.Id);
        if (staff is null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }

        if (Equals(staff.Status, request.Status) || Equals(request.Status, AccountStatus.Verifying))
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