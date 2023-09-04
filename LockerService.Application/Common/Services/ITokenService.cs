namespace LockerService.Application.Common.Services;

public interface ITokenService
{
    Task SetInvalidateTokenJob(Token token);
}