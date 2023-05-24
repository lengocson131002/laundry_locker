namespace LockerService.Application.Hardwares.Handlers;

public class AddHardwareHandler : IRequestHandler<AddHardwareCommand, HardwareResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddHardwareHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<HardwareResponse> Handle(AddHardwareCommand request, CancellationToken cancellationToken)
    {
        var lockerQuery = await _unitOfWork.LockerRepository.GetAsync(
            predicate: locker => locker.Id == request.LockerId,
            includes: new List<Expression<Func<Locker, object>>>()
            {
                locker => locker.Hardwares
            });

        var locker = lockerQuery.FirstOrDefault();
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        var hardware = _mapper.Map<Hardware>(request);
        locker.Hardwares.Add(hardware);

        await _unitOfWork.LockerRepository.UpdateAsync(locker);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<HardwareResponse>(hardware);
    }
}