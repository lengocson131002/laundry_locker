using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Lockers.Models;
using LockerService.Application.Features.Lockers.Queries;

namespace LockerService.Application.Features.Lockers.Handlers;

public class GetAllBoxesHandler : IRequestHandler<GetAllBoxesQuery, ListResponse<BoxResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBoxesHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ListResponse<BoxResponse>> Handle(GetAllBoxesQuery request, CancellationToken cancellationToken)
    {
        var locker = await _unitOfWork.LockerRepository.GetByIdAsync(request.LockerId);
        if (locker == null)
        {
            throw new ApiException(ResponseCode.LockerErrorNotFound);
        }

        var boxes = await _unitOfWork.BoxRepository.GetAllBoxes(request.LockerId);
        return new ListResponse<BoxResponse>(_mapper.Map<IList<Box>, IList<BoxResponse>>(boxes));
    }
}