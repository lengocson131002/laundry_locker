using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Domain.Enums;
using Quartz;

namespace LockerService.Infrastructure.Scheduler;

public class InvalidateTokenJob : IJob
{
    public const string InvalidateTokenJobKey = "InvalidateTokenJobKey";

    public const string TokenIdKey = "TokenId";

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<InvalidateTokenJob> _logger;

    public InvalidateTokenJob(IUnitOfWork unitOfWork, ILogger<InvalidateTokenJob> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.JobDetail.JobDataMap;

        var tokenId = dataMap.GetLongValue(TokenIdKey);

        if (tokenId == 0)
        {
            return;
        }

        var token = await _unitOfWork.TokenRepository
            .Get(token => token.Id == tokenId && !Equals(token.Status, TokenStatus.Invalid))
            .FirstOrDefaultAsync();

        if (token == null)
        {
            return;
        }

        token.Status = TokenStatus.Invalid;
        await _unitOfWork.TokenRepository.UpdateAsync(token);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Invalidate expired token {tokenId} at {time}", tokenId, token.ExpiredAt);
    }
}