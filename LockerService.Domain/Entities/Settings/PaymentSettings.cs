namespace LockerService.Domain.Entities.Settings;

public class PaymentSettings : ISetting
{
    // Timeout for an payment. Payment will be failed if not completed after this period of time
    public int PaymentTimeoutInMinutes { get; set; }

    // Minimum allowed deposit amount of money
    public decimal MinDeposit { get; set; }

    public PaymentSettings()
    {
        PaymentTimeoutInMinutes = 10;
        MinDeposit = 10000;
    }
}