namespace LockerService.Application.Auth.Handlers;

public class UpdateCustomerProfileHandler : IRequestHandler<UpdateCustomerProfileCommand, CustomerResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly IMapper _mapper;

    private readonly ICurrentAccountService _currentAccountService;

    public UpdateCustomerProfileHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentAccountService currentAccountService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentAccountService = currentAccountService;
    }

    public async Task<CustomerResponse> Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        var currentCustomer = await _currentAccountService.GetCurrentAccount();

        if (currentCustomer == null)
        {
            throw new ApiException(ResponseCode.Unauthorized);
        }

        currentCustomer.Avatar = request.Avatar ?? currentCustomer.Avatar;
        currentCustomer.FullName = request.FullName ?? currentCustomer.FullName;
        currentCustomer.Description = request.Description ?? currentCustomer.Description;

        await _unitOfWork.AccountRepository.UpdateAsync(currentCustomer);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CustomerResponse>(currentCustomer);
    }
}