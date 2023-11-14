using MediatR;

namespace LockerService.API.Controllers;

/// <summary>
/// B
/// </summary>
public class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    /// <summary>
    /// 
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}