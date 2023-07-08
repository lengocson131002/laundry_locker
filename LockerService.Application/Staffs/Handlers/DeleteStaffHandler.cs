namespace LockerService.Application.Staffs.Handlers;

public class DeleteStaffHandler : IRequestHandler<DeleteStaffCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<DeleteStaffHandler> _logger;
    private readonly IMapper _mapper;

    public DeleteStaffHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<DeleteStaffHandler> logger,
        IJwtService jwtService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<StatusResponse> Handle(DeleteStaffCommand request, CancellationToken cancellationToken)
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
            await _unitOfWork.AccountRepository.GetAsync(a =>
                a.PhoneNumber != null && Equals(a.Id, request.Id));

        var account = accountQuery.FirstOrDefault();

        if (account is null)
        {
            throw new ApiException(ResponseCode.StaffErrorNotFound);
        }

        account.Store = null;

        await _unitOfWork.AccountRepository.UpdateAsync(account);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return new StatusResponse(true);
    }
}