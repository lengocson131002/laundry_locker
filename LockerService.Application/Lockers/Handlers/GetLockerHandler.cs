namespace LockerService.Application.Lockers.Handlers;

public class GetLockerHandler :
    IRequestHandler<GetLockerQuery, LockerDetailResponse>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetLockerHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<LockerDetailResponse> Handle(GetLockerQuery request, CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.LockerRepository.GetAsync(
            predicate: locker => locker.Id == request.LockerId,
            includes: new List<Expression<Func<Locker, object>>>()
            {
                locker => locker.Location,
                locker => locker.Location.Province,
                locker => locker.Location.District,
                locker => locker.Location.Ward,
                locker => locker.Store,
                locker => locker.Store.Location,
                locker => locker.Store.Location.Province,
                locker => locker.Store.Location.District,
                locker => locker.Store.Location.Ward,
                locker => locker.OrderTypes,
            },
            disableTracking: true
        );
        

        var locker = query.FirstOrDefault();
        if (locker is null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        Console.WriteLine(locker.Location.ToString());
        return _mapper.Map<LockerDetailResponse>(locker);
    }
}