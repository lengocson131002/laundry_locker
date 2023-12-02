using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Features.Customers.Models;
using LockerService.Application.Features.Customers.Queries;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Customers.Handlers;

public class GetCustomerByPhoneHandler : IRequestHandler<GetCustomerByPhoneQuery, CustomerDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCustomerByPhoneHandler> _logger;

    public GetCustomerByPhoneHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        ILogger<GetCustomerByPhoneHandler> 
            logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CustomerDetailResponse> Handle(GetCustomerByPhoneQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork
            .AccountRepository
            .GetCustomerByPhoneNumber(request.Phone.NormalizePhoneNumber());
        
        if (customer == null)
        {
            _logger.LogInformation($"[QUERY CUSTOMER BY PHONE] Customer not found with phne number {request.Phone}. Create new account for customer with phone number {request.Phone}");
            customer = new Account()
            {
                Role = Role.Customer,
                PhoneNumber = request.Phone,
                Status = AccountStatus.Active,
                Wallet = new Wallet()
            };

            await _unitOfWork.AccountRepository.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
        }
        else if (customer.Wallet == null)
        {
            _logger.LogInformation($"[QUERY CUSTOMER BY PHONE] Customer's wallet not found. Create wallet for customer with phone number {request.Phone}");
            var wallet = new Wallet();
            customer.Wallet = wallet;
            await _unitOfWork.WalletRepository.AddAsync(wallet);
            await _unitOfWork.SaveChangesAsync();
        }

        return _mapper.Map<CustomerDetailResponse>(customer);
    }
}