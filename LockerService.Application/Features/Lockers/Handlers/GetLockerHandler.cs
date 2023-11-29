using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Lockers.Models;
using LockerService.Application.Features.Lockers.Queries;

namespace LockerService.Application.Features.Lockers.Handlers;

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
        var locker = await _unitOfWork.LockerRepository.Get(
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
        ).FirstOrDefaultAsync(cancellationToken);
        
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }
        
        var boxes = await _unitOfWork.BoxRepository.GetAllBoxes(request.LockerId);
        var response = _mapper.Map<LockerDetailResponse>(locker);
        response.BoxCount = boxes.Count;
        response.AvailableBoxCount = boxes.Count(box => box.IsAvailable);

        return response;
        
    }
}