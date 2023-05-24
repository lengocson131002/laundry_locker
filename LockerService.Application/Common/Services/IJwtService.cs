namespace LockerService.Application.Common.Services;

public interface IJwtService
{
    string GenerateJwtToken(Account account);
    
    string GenerateJwtRefreshToken(Account account);

}