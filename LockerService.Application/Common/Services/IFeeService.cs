namespace LockerService.Application.Common.Services;

public interface IFeeService
{
    double CalculateFree(in Order order);
}