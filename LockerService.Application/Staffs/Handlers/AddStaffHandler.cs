using LockerService.Application.Common.Security;
using LockerService.Application.EventBus.RabbitMq.Events.Accounts;

namespace LockerService.Application.Staffs.Handlers;

public class AddStaffHandler : IRequestHandler<AddStaffCommand, StaffDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AddStaffHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRabbitMqBus _rabbitMqBus;

    public AddStaffHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<AddStaffHandler> logger,
        IJwtService jwtService, IRabbitMqBus rabbitMqBus)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task<StaffDetailResponse> Handle(AddStaffCommand request, CancellationToken cancellationToken)
    {
        var storeQuery =
            await _unitOfWork.StoreRepository.GetAsync(s => Equals(s.Id, request.StoreId));
        
        var store = storeQuery.FirstOrDefault();
        if (store == null)
        {
            throw new ApiException(ResponseCode.StoreErrorNotFound);
        }

        var staff = await _unitOfWork.AccountRepository
            .GetByUsername(request.Username)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (staff != null)
        {
            throw new ApiException(ResponseCode.AccountErrorUsernameExisted);
        }
        
        staff = new Account()
        {
            Username = request.Username,
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            Avatar = request.Avatar,
            Password = BCryptUtils.Hash(request.Password),
            Description = request.Description,
            Status = AccountStatus.Verifying,
            Role = request.Role,
            Store = store,
        };

        await _unitOfWork.AccountRepository.AddAsync(staff);

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        
        // Push RabbitMQ event
        await _rabbitMqBus.PublishAsync(new StaffCreatedEvent()
        {
            AccountId = staff.Id,
            PhoneNumber = staff.PhoneNumber,
            Username = staff.Username,
            Password = request.Password,
            FullName = staff.FullName,
            Role = staff.Role
        }, cancellationToken);
            
        return _mapper.Map<StaffDetailResponse>(staff);
    }
}