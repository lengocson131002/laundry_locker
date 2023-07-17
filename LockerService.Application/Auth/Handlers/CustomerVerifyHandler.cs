using LockerService.Application.Common.Utils;

namespace LockerService.Application.Auth.Handlers;

public class CustomerVerifyHandler : IRequestHandler<CustomerVerifyRequest, StatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<CustomerVerifyHandler> _logger;

    public CustomerVerifyHandler(IUnitOfWork unitOfWork, ILogger<CustomerVerifyHandler> logger, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<StatusResponse> Handle(CustomerVerifyRequest request, CancellationToken cancellationToken)
    {
        var accountQuery = await _unitOfWork.AccountRepository.GetAsync(
            predicate: account => Equals(request.Username.ToLower(), account.Username)
        );

        var account = accountQuery.FirstOrDefault();
        if (account is null || !Equals(account.Role, Role.Customer))
        {
            account = new Account
            {
                Username = request.Username,
                FullName = request.Username,
                PhoneNumber = request.Username,
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
        // TODO: Handle send OTP

        await _unitOfWork.SaveChangesAsync();
        return new StatusResponse(true);
    }
}