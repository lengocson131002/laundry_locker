using LockerService.API.Attributes;
using LockerService.Application.Features.Payments.Models;
using LockerService.Application.Features.Wallets.Commands;

namespace LockerService.API.Controllers;

/// <summary>
/// WALLET APIS
/// </summary>
[ApiController]
[Route("/api/v1/wallets/")]
[ApiKey]
public class WalletController : ApiControllerBase
{
    /// <summary>
    /// API for customer to deposit
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("deposit")]
    public async Task<PaymentResponse> Deposit([FromBody] DepositCommand command)
    {
        return await Mediator.Send(command);
    }
}