using LockerService.Application.Common.Enums;
using LockerService.Application.Orders.Commands;
using LockerService.Application.Orders.Models;
using LockerService.Application.Orders.Queries;

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

    [HttpPut("{id:long}")]
    public async Task<ActionResult<OrderResponse>> UpdateOrder([FromRoute] long id, [FromBody] UpdateOrderCommand request)
    {
        request.Id = id;
        return await Mediator.Send(request);
    }

    [HttpPut("{id:long}/confirm")]
    public async Task<ActionResult> ConfirmOrder([FromRoute] long id)
    {
        var confirmOrderRequest = new ConfirmOrderCommand
        {
            Id = id
        };

        await Mediator.Send(confirmOrderRequest);
        return Ok();
    }

    [HttpPut("{id:long}/process")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<OrderResponse>> ProcessOrder([FromRoute] long  id)
    {
        var command = new ProcessOrderCommand(id);
        return await Mediator.Send(command);
    }

    [HttpPut("{id:long}/return")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<OrderResponse>> ReturnOrder([FromRoute] long id)
    {
        var command = new ReturnOrderCommand(id);
        return await Mediator.Send(command);
    }
    
    [HttpPut("{id:long}/checkout")]
    public async Task<ActionResult<OrderResponse>> CheckoutOrder([FromRoute] long id)
    {
        var command = new CheckoutOrderCommand(id);
        return await Mediator.Send(command);
    }

    [HttpGet("{id:long}")]
    [Authorize(Roles = "Staff,Customer")]
    public async Task<ActionResult<OrderDetailResponse>> GetOrder([FromRoute] long id)
    {
        var getOrderRequest = new GetOrderQuery
        {
            Id = id
        };

        return await Mediator.Send(getOrderRequest);
    }
    
    [HttpGet("pin-code/{pinCode}")]
    public async Task<ActionResult<OrderDetailResponse>> GetOrder([FromRoute] string pinCode)
    {
        var getOrderRequest = new GetOrderByPinCodeQuery(pinCode);
        return await Mediator.Send(getOrderRequest);
    }
    
    [HttpGet]
    [Authorize(Roles = "Staff,Customer")]
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
    
    [HttpGet("{id:long}/details/{detailId:long}")]
    [Authorize(Roles = "Staff,Customer")]
    public async Task<ActionResult<OrderItemResponse>> GetOrderDetail([FromRoute] long id, [FromRoute] long detailId)
    {
        var request = new GetOrderDetailQuery(id, detailId);
        return await Mediator.Send(request);
    }
    
    [HttpDelete("{id:long}/details/{detailId:long}")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<OrderItemResponse>> RemoveOrderDetail([FromRoute] long id, [FromRoute] long detailId)
    {
        var request = new RemoveOrderDetailCommand(id, detailId);
        return await Mediator.Send(request);
    }
    
    [HttpPut("{id:long}/details/{detailId:long}")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<OrderItemResponse>> UpdateOrderDetail([FromRoute] long id, [FromRoute] long detailId)
    {
        var request = new RemoveOrderDetailCommand(id, detailId);
        return await Mediator.Send(request);
    }
}