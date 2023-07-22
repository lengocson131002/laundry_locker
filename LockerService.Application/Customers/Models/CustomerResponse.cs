namespace LockerService.Application.Customers.Models;

public class CustomerResponse : AccountResponse
{
    public int OrderCount { get; set; }
}