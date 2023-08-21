using LockerService.Infrastructure.Common.Constants;

namespace LockerService.Infrastructure.HttpClients;

public class RetryHandler : DelegatingHandler
{
    public RetryHandler(HttpMessageHandler innerHandler) : base(innerHandler) {}

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = null;
        for (int i = 0; i < HttpClientConstants.MaxRetries; i++)
        {
            response = await base.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
        }

        return response;
    }
}