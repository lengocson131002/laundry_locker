using LockerService.Application.Common.Services.Payments;
using LockerService.Domain;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;

namespace LockerService.Infrastructure.Services.Payments.VnPay;

public class VnPayPaymentService : IVnPayPaymentService
{
    private readonly VnPaySettings _vnPaySettings;

    private readonly IHttpContextAccessor _contextAccessor;

    private const string PayCommand = "pay";

    private const string RefundCommand = "refund";

    private const string CurrCode = "VND";

    private const string Locale = "vn";

    private const string DefaultPaymentInfo = "Thanh toán với VnPay";
    
    public VnPayPaymentService(VnPaySettings vnPaySettings, IHttpContextAccessor contextAccessor)
    {
        _vnPaySettings = vnPaySettings;
        _contextAccessor = contextAccessor;
    }

    public async Task<Payment> CreatePayment(VnPayPayment payment)
    {
        HttpContext? context = _contextAccessor.HttpContext;
        if (context == null)
        {
            throw new Exception("Http Context not found");
        }
        
        var pay = new VnPayLibrary();
        var urlCallBack = $"{_vnPaySettings.CallbackUrl}/{payment.PaymentReferenceId}";

        pay.AddRequestData("vnp_Version", _vnPaySettings.Version);
        pay.AddRequestData("vnp_Command", PayCommand);
        pay.AddRequestData("vnp_TmnCode", _vnPaySettings.TmnCode);
        pay.AddRequestData("vnp_Amount", ((int)payment.Amount * 100).ToString());
        pay.AddRequestData("vnp_CreateDate", payment.Time.ToString("yyyyMMddHHmmss"));
        pay.AddRequestData("vnp_CurrCode", CurrCode);
        pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
        pay.AddRequestData("vnp_Locale", Locale);
        pay.AddRequestData("vnp_OrderInfo", payment.Info ?? DefaultPaymentInfo);
        pay.AddRequestData("vnp_OrderType", payment.OrderType.ToString());
        pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
        pay.AddRequestData("vnp_TxnRef", payment.PaymentReferenceId);
        pay.AddRequestData("vnp_BankCode", string.Empty);

        var paymentUrl =
            pay.CreateRequestUrl(_vnPaySettings.PaymentEndpoint, _vnPaySettings.HashSecret);

        var pm = new Payment()
        {
            Method = PaymentMethod.VnPay,
            Amount = payment.Amount,
            Content = payment.Info,
            ReferenceId = payment.PaymentReferenceId,
            Url = paymentUrl,
            Qr = paymentUrl
        };

        return await Task.FromResult(pm);
    }
}