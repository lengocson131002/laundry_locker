using System.Security.Cryptography;
using System.Text;
using LockerService.Application.Common.Services.Payments;
using LockerService.Domain;
using LockerService.Domain.Enums;
using LockerService.Infrastructure.HttpClients;
using LockerService.Infrastructure.Settings;
using LockerService.Shared.Utils;

namespace LockerService.Infrastructure.Services.Payments.Momo;

public class MomoPaymentService : IMomoPaymentService
{
    private readonly ILogger<MomoPaymentService> _logger;
    
    private const string DefaultOrderInfo = "[Laundry locker] Thanh toán với Momo";

    private readonly MomoSettings _momoSettings;


    public MomoPaymentService(MomoSettings momoSettings, ILogger<MomoPaymentService> logger)
    {
        _momoSettings = momoSettings;
        _logger = logger;
    }


    public async Task<string> CreateQrCodeUrl(MomoPayment payment)
    {
        var momoPaymentResponse = await CreatePayment(payment);
        return momoPaymentResponse.Qr ?? string.Empty;
    }

    public async Task<string> CreatePaymentUrl(MomoPayment payment)
    {
        var momoPaymentResponse = await CreatePayment(payment);
        return  momoPaymentResponse.Url ?? string.Empty;
    }

    public async Task<string> CreateDeeplink(MomoPayment payment)
    {
        var momoPaymentResponse = await CreatePayment(payment);
        return momoPaymentResponse.Deeplink ?? string.Empty;
    }

    public async Task<Payment> CreatePayment(MomoPayment payment)
    {
        var requestType = "captureWallet";
        var request = new MomoPaymentRequest();
        request.OrderInfo = payment.Info ?? DefaultOrderInfo;
        request.PartnerCode = _momoSettings.PartnerCode;
        request.IpnUrl = $"{_momoSettings.IpnUrl}/{payment.PaymentReferenceId}";
        request.RedirectUrl = $"{_momoSettings.RedirectUrl}/{payment.PaymentReferenceId}";
        request.Amount = payment.Amount;
        request.OrderId = payment.OrderReferenceId;
        request.ReferenceId = $"{payment.PaymentReferenceId}";
        request.RequestId = Guid.NewGuid().ToString();
        request.RequestType = requestType;
        request.ExtraData = string.Empty;
        request.AutoCapture = true;
        request.Lang = "vi";
        
        var rawSignature = $"accessKey={_momoSettings.AccessKey}&amount={request.Amount}&extraData={request.ExtraData}&ipnUrl={request.IpnUrl}&orderId={request.OrderId}&orderInfo={request.OrderInfo}&partnerCode={request.PartnerCode}&redirectUrl={request.RedirectUrl}&requestId={request.RequestId}&requestType={requestType}";
        request.Signature = GetSignature(rawSignature, _momoSettings.SecretKey);

        var httpContent = new StringContent(JsonSerializerUtils.Serialize(request),Encoding.UTF8, "application/json");
        using var httpClient = new HttpClient(new RetryHandler());
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        var momoResponse = await httpClient.PostAsync(_momoSettings.PaymentEndpoint, httpContent);
        var responseContent = momoResponse.Content.ReadAsStringAsync().Result;

        if (momoResponse.IsSuccessStatusCode)
        {
            var momoPaymentResponse = JsonSerializerUtils.Deserialize<MomoPaymentResponse>(responseContent);
            if (momoPaymentResponse != null)
            {
                var pm =  new Payment()
                {
                    Method = PaymentMethod.Momo,
                    Amount = momoPaymentResponse.Amount,
                    // Test with QR
                    Qr = momoPaymentResponse.PayUrl,
                    Url = momoPaymentResponse.PayUrl,
                    Deeplink = momoPaymentResponse.Deeplink,
                    ReferenceId = payment.PaymentReferenceId,
                    Content = payment.Info,
                    Status = PaymentStatus.Created,
                    Description = "Payment with momo"
                };

                return pm;
            }
        }

        throw new Exception($"[Momo payment] Error: There is some error when create payment with momo. {responseContent}");
    }


    private static string GetSignature(string text, string key)
    {
        // change according to your needs, an UTF8Encoding
        // could be more suitable in certain situations
        var encoding = new UTF8Encoding();

        var textBytes = encoding.GetBytes(text);
        var keyBytes = encoding.GetBytes(key);

        Byte[] hashBytes;

        using HMACSHA256 hash = new HMACSHA256(keyBytes);
        hashBytes = hash.ComputeHash(textBytes);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}