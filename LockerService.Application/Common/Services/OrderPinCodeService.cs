namespace LockerService.Application.Common.Services;

public class OrderPinCodeService
{
    private const string AllowedCharacters = "0123456789";

    private readonly IUnitOfWork _unitOfWork;

    public OrderPinCodeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<string> GenerateUniqueOrderPinCode(int length = 6)
    {
        while (true)
        {
            var pinCode = GeneratePinCode(length);
            
            var orderQuery = await _unitOfWork.OrderRepository.GetAsync(
                order => pinCode.Equals(order.PinCode)
                         && !OrderStatus.Completed.Equals(order.Status)
                         && !OrderStatus.Canceled.Equals(order.Status));

            if (!orderQuery.Any()) return pinCode;
            
        }
    }

    private string GeneratePinCode(int length)
    {
        var rand = new Random();
        
        var otp = string.Empty;

        for (var i = 0; i < length; i++)
        {
            otp += AllowedCharacters[rand.Next(0, AllowedCharacters.Length)];
        }

        return otp;
    }
}