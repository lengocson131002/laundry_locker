using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LockerService.API.Controllers;

public class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}