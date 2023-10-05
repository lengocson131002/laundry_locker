using LockerService.Application.EventBus.RabbitMq.Events.Accounts;

namespace LockerService.Application.Auth.Handlers;

public class CustomerVerifyHandler : IRequestHandler<CustomerVerifyRequest, OtpResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<CustomerVerifyHandler> _logger;
    private readonly IRabbitMqBus _rabbitMqBus;

    public CustomerVerifyHandler(IUnitOfWork unitOfWork, ILogger<CustomerVerifyHandler> logger, IJwtService jwtService, IRabbitMqBus rabbitMqBus)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
        _rabbitMqBus = rabbitMqBus;
    }

    public async Task<OtpResponse> Handle(CustomerVerifyRequest request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.AccountRepository.GetCustomerByPhoneNumber(request.PhoneNumber);
        if (account is not null && !Equals(account.Status, AccountStatus.Active))
        {
            throw new ApiException(ResponseCode.AuthErrorAccountInactive);
        }

        if (account is null)
        {
            account = new Account
            {
                Username = request.PhoneNumber,
                FullName = request.PhoneNumber,
                PhoneNumber = request.PhoneNumber,
                Status = AccountStatus.Active,
                Role = Role.Customer
            };
            await _unitOfWork.AccountRepository.AddAsync(account);
        }

        var tokenQuery = await _unitOfWork.TokenRepository.GetAsync(
            predicate: token => token.AccountId == account.Id && Equals(token.Type, TokenType.Otp) &&
                                Equals(token.Status, TokenStatus.Valid)
        );

        // invalid old token
        await tokenQuery.ForEachAsync(async token =>
        {
            token.Status = TokenStatus.Invalid;
            await _unitOfWork.TokenRepository.UpdateAsync(token);
        }, cancellationToken: cancellationToken);

        var token = new Token
        {
            Account = account,
            Status = TokenStatus.Valid,
            Type = TokenType.Otp,
            Value = GeneratorUtils.GenerateToken(6)
        };
        await _unitOfWork.TokenRepository.AddAsync(token);
        await _unitOfWork.SaveChangesAsync();

        await _rabbitMqBus.PublishAsync(new OtpCreatedEvent()
        {
            AccountId = account.Id,
            Otp = token.Value
        }, cancellationToken);
        
        return new OtpResponse
        {
            Otp = token.Value
        };
    }
}