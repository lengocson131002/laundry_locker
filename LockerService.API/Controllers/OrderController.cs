using LockerService.Application.Common.Enums;
using LockerService.Application.Orders.Commands;
using LockerService.Application.Orders.Models;
using LockerService.Application.Orders.Queries;
using LockerService.Domain.Entities;

namespace LockerService.API.Controllers;

[ApiController]
[Route("/api/v1/orders")]
public class OrderController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        return await Mediator.Send(command);
    }
    
    [HttpPost("reservations")]
    public async Task<ActionResult<OrderResponse>> ReserveOrder([FromBody] ReserveOrderCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("reservations")]
    public async Task<ActionResult<OrderResponse>> TakeReservationOrder([FromQuery] TakeReservationCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrderResponse>> UpdateOrder([FromRoute] int id,
        [FromBody] UpdateOrderCommand request)
    {
        request.Id = id;
        return await Mediator.Send(request);
    }

    [HttpPut("{id:int}/confirm")]
    public async Task<ActionResult<OrderResponse>> ConfirmOrder([FromRoute] int id)
    {
        var confirmOrderRequest = new ConfirmOrderCommand
        {
            Id = id
        };

        return await Mediator.Send(confirmOrderRequest);
    }

    [HttpPut("{id:int}/process")]
    public async Task<ActionResult<OrderResponse>> ProcessOrder([FromRoute] int id)
    {
        var processOrderRequest = new ProcessOrderCommand
        {
            Id = id
        };

        return await Mediator.Send(processOrderRequest);
    }

    [HttpPut("{id:int}/return")]
    public async Task<ActionResult<OrderResponse>> ReturnOrder([FromRoute] int id, [FromBody] ReturnOrderCommand request)
    {
        request.Id = id;
        return await Mediator.Send(request);
    }
    
    [HttpPut("checkout")]
    public async Task<ActionResult<OrderResponse>> CheckoutOrder([FromQuery] CheckoutOrderCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDetailResponse>> GetOrder([FromRoute] int id)
    {
        var getOrderRequest = new GetOrderQuery
        {
            Id = id
        };

        return await Mediator.Send(getOrderRequest);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponse<Order, OrderResponse>>> GetOrders(
        [FromQuery] GetAllOrdersQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.SortColumn))
        {
            request.SortColumn = "CreatedAt";
            request.SortDir = SortDirection.Desc;
        }
        return await Mediator.Send(request);
    }
}