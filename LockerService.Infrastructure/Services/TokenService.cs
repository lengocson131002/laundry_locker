using LockerService.Application.Common.Persistence.Repositories;
using LockerService.Infrastructure.Scheduler;
using Quartz;

namespace LockerService.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<TokenService> _logger;
    
    private readonly ISchedulerFactory _schedulerFactory;

    public TokenService(IUnitOfWork unitOfWork, ISchedulerFactory schedulerFactory, ILogger<TokenService> logger)
    {
        _unitOfWork = unitOfWork;
        _schedulerFactory = schedulerFactory;
        _logger = logger;
    }

    public async Task SetInvalidateTokenJob(Token token)
    {
        try
        {
            if (token.ExpiredAt == null || token.IsExpired)
            {
                return;
            }
            
            _logger.LogInformation("Token {orderId} will be expired at {time}", token.Id, token.ExpiredAt);
            
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<InvalidateTokenJob>()
                .UsingJobData(InvalidateTokenJob.TokenIdKey, token.Id)
                .Build();
            
            var trigger = TriggerBuilder.Create()
                .StartAt(token.ExpiredAt.Value)
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
        catch (Exception ex)
        {
            _logger.LogError("Schedule to invalidate expired token error {error}", ex.Message);
        }
    }
}