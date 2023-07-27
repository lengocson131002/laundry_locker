namespace LockerService.Application.Lockers.Handlers;

public class AddBoxHandler : IRequestHandler<AddBoxCommand, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddBoxHandler> _logger;

    public AddBoxHandler(IUnitOfWork unitOfWork, ILogger<AddBoxHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<StatusResponse> Handle(AddBoxCommand request, CancellationToken cancellationToken)
    {
        var lockerId = request.LockerId;
        var boxNumber = request.BoxNumber;
        
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(lockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        var existBox = await _unitOfWork.BoxRepository.FindBox(lockerId, boxNumber);
        if (existBox == null)
        {
            var box = new Box(lockerId, boxNumber);
            await _unitOfWork.BoxRepository.AddAsync(box);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Locker {0} is added new box {1}", lockerId, boxNumber);
        } else if (!existBox.IsActive)
        {
            existBox.IsActive = true;
            await _unitOfWork.BoxRepository.UpdateAsync(existBox);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Locker {0} is updated box {1}", lockerId, boxNumber);
        }

        return new StatusResponse(true);
    }
}