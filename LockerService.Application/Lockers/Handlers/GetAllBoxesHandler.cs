namespace LockerService.Application.Lockers.Handlers;

public class GetAllBoxesHandler : IRequestHandler<GetAllBoxesQuery, ListResponse<BoxStatus>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBoxesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ListResponse<BoxStatus>> Handle(GetAllBoxesQuery request, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(request.LockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        return new ListResponse<BoxStatus>(await _unitOfWork.LockerRepository.GetAllBoxes(request.LockerId));
    }
}