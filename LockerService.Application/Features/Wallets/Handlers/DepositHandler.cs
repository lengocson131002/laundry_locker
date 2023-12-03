using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Application.Common.Services.Payments;
using LockerService.Application.Features.Payments.Models;
using LockerService.Application.Features.Wallets.Commands;
using LockerService.Domain;
using LockerService.Domain.Entities.Settings;
using LockerService.Shared.Constants;
using LockerService.Shared.Extensions;

namespace LockerService.Application.Features.Wallets.Handlers;

public class DepositHandler : IRequestHandler<DepositCommand, PaymentResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly IPaymentService _paymentService;

    private readonly ILogger<DepositHandler> _logger;

    private readonly ISettingService _settingService;

    public DepositHandler(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IPaymentService paymentService, 
        ILogger<DepositHandler> logger, 
        ISettingService settingService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _paymentService = paymentService;
        _logger = logger;
        _settingService = settingService;
    }

    public async Task<PaymentResponse> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        // Get payment settings
        var paymentSettings = await _settingService.GetSettings<PaymentSettings>(cancellationToken);

        // Validate deposit amount
        var minDeposit = paymentSettings.MinDeposit;
        if (request.Amount < minDeposit)
        {
            throw new ApiException(
                ResponseCode.WalletErrorInvalidDepositAmount, 
                string.Format(ResponseCode.WalletErrorInvalidDepositAmount.GetDescription(), paymentSettings.MinDeposit));
        }
        
        // Get customer account
        var customer = await _unitOfWork.AccountRepository.GetCustomerByPhoneNumber(request.PhoneNumber);
        if (customer != null)
        {
            // Validate customer status
            if (!customer.IsActive)
            {
                throw new ApiException(ResponseCode.OrderErrorInactiveAccount);
            }

            // Create customer wall if not exist
            if (customer.Wallet == null)
            {
                _logger.LogInformation("Customer's wallet not found. Create wallet");
                customer.Wallet = new Wallet();
                await _unitOfWork.WalletRepository.AddAsync(customer.Wallet);
            }
        }

        if (customer == null)
        {
            _logger.LogInformation("Customer not found. Create customer account");
            customer = new Account();
            customer.Role = Role.Customer;
            customer.PhoneNumber = request.PhoneNumber;
            customer.Wallet = new Wallet();
            
            await _unitOfWork.AccountRepository.AddAsync(customer);
        }
        
        // Create payment
        var payment = await CreateDepositPayment(customer, request.Amount, request.Method);
        await _unitOfWork.PaymentRepository.AddAsync(payment);
        
        // Save all changes
        await _unitOfWork.SaveChangesAsync();
        
        // Set timeout to clear payment
        var timeoutAt = DateTimeOffset.UtcNow.AddMinutes(paymentSettings.PaymentTimeoutInMinutes);
        await _paymentService.SetPaymentTimeOut(payment, timeoutAt);
        
        return _mapper.Map<PaymentResponse>(payment);
    }

    private async Task<Payment> CreateDepositPayment(Account customer, decimal amount, PaymentMethod method)
    {
        Payment payment = null;
        switch (method)
        {
            case PaymentMethod.Momo:
            {
                payment = await _paymentService.CreatePayment(new MomoPayment()
                {
                    PaymentReferenceId = Guid.NewGuid().ToString(),
                    Amount = (long)amount,
                    Info = PaymentType.Deposit.GetDescription()
                });
                break;
            }

            case PaymentMethod.VnPay:
            {
                payment = await _paymentService.CreatePayment(new VnPayPayment()
                {
                    PaymentReferenceId = Guid.NewGuid().ToString(),
                    Amount = (long)amount,
                    Info = PaymentType.Deposit.GetDescription(),
                    OrderType = PaymentType.Deposit.GetDescription(),
                    Time = DateTimeOffset.UtcNow
                });
                break;
            }
        
            default:
                throw new ApiException(ResponseCode.PaymentErrorUnsupportedPaymentMethod);
        }

        payment.Customer = customer;
        payment.Type = PaymentType.Deposit;
        
        return payment;
    }
}