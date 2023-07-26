using LockerService.Application.Settings.Models;
using LockerService.Application.Settings.Queries;

namespace LockerService.Application.Settings.Handlers;

public class GetAllSettingsHandler : IRequestHandler<GetAllSettingsQuery, ListResponse<SettingResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSettingsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ListResponse<SettingResponse>> Handle(GetAllSettingsQuery request, CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.SettingRepository.GetAsync(
            predicate: request.GetExpressions(),
            orderBy: request.GetOrder(),
            disableTracking: true);

        var settingResponses = _mapper.Map<List<Setting>, List<SettingResponse>>(query.ToList());
        return new ListResponse<SettingResponse>(settingResponses);
    }
}