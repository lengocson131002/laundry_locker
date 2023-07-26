namespace LockerService.Application.Common.Services;

public interface IFeeService
{
    Task CalculateFree(Order order);
    
}