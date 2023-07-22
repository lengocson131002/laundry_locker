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

    [HttpPut("{id:long}")]
    public async Task<ActionResult<OrderResponse>> UpdateOrder([FromRoute] long id, [FromBody] UpdateOrderCommand request)
    {
        request.Id = id;
        return await Mediator.Send(request);
    }

    [HttpPut("{id:long}/confirm")]
    public async Task<ActionResult<OrderResponse>> ConfirmOrder([FromRoute] long id)
    {
        var confirmOrderRequest = new ConfirmOrderCommand
        {
            Id = id
        };

        return await Mediator.Send(confirmOrderRequest);
    }

    [HttpPut("process")]
    public async Task<ActionResult<OrderResponse>> ProcessOrder([FromQuery] ProcessOrderCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("return")]
    public async Task<ActionResult<OrderResponse>> ReturnOrder([FromRoute] ReturnOrderCommand command)
    {
        return await Mediator.Send(command);
    }
    
    [HttpPut("checkout")]
    public async Task<ActionResult<OrderResponse>> CheckoutOrder([FromQuery] CheckoutOrderCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<OrderDetailResponse>> GetOrder([FromRoute] long id)
    {
        var getOrderRequest = new GetOrderQuery
        {
            Id = id
        };

        return await Mediator.Send(getOrderRequest);
    }
    
    [HttpGet("pin-code")]
    public async Task<ActionResult<OrderDetailResponse>> GetOrder([FromRoute] string pinCode)
    {
        var getOrderRequest = new GetOrderByPinCodeQuery(pinCode);
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
    
    [HttpGet("{id:long}/details/{detailId:long}")]
    public async Task<ActionResult<OrderItemResponse>> GetOrderDetail([FromRoute] long id, [FromRoute] long detailId)
    {
        var request = new GetOrderDetailQuery(id, detailId);
        return await Mediator.Send(request);
    }
    
    [HttpDelete("{id:long}/details/{detailId:long}")]
    public async Task<ActionResult<OrderItemResponse>> RemoveOrderDetail([FromRoute] long id, [FromRoute] long detailId)
    {
        var request = new RemoveOrderDetailCommand(id, detailId);
        return await Mediator.Send(request);
    }
    
    [HttpPut("{id:long}/details/{detailId:long}")]
    public async Task<ActionResult<OrderItemResponse>> UpdateOrderDetail([FromRoute] long id, [FromRoute] long detailId)
    {
        var request = new RemoveOrderDetailCommand(id, detailId);
        return await Mediator.Send(request);
    }
}