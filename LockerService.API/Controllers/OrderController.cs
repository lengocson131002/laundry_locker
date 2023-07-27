using LockerService.Application.Bills.Models;
using LockerService.Application.Bills.Queries;
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
    [Authorize]
    public async Task<ActionResult<OrderResponse>> ReserveOrder([FromBody] ReserveOrderCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<OrderResponse>> UpdateOrder([FromRoute] long id, [FromBody] UpdateOrderCommand request)
    {
        request.Id = id;
        return await Mediator.Send(request);
    }
    
    [HttpPut("{id:long}/reservation")]
    public async Task<ActionResult<OrderResponse>> UseOrderReservation([FromRoute] long id)
    {
        var command = new TakeReservationCommand()
        {
            Id = id
        };
        return await Mediator.Send(command);
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
    public async Task<ActionResult<BillResponse>> CheckoutOrder([FromRoute] long id, [FromBody] CheckoutOrderCommand command)
    {
        command.Id = id;
        return await Mediator.Send(command);
    }

    [HttpGet("{id:long}/checkout/callback")]
    public async Task<ActionResult> CheckoutOrderCallBack([FromRoute] long id, [FromBody] CheckoutOrderCallbackCommand command)
    {
        command.Id = id;
        await Mediator.Send(command);
        return Ok();
    }

    [HttpGet("{id:long}")]
    [Authorize]
    public async Task<ActionResult<OrderDetailResponse>> GetOrder([FromRoute] long id)
    {
        var getOrderRequest = new GetOrderQuery
        {
            Id = id
        };

        return await Mediator.Send(getOrderRequest);
    }
    
    [HttpGet("{id:long}/bill")]
    public async Task<ActionResult<BillResponse>> GetBill([FromRoute] long id)
    {
        var query = new BillQuery(id);
        return await Mediator.Send(query);
    }
    
    [HttpGet("pin-code/{pinCode}")]
    public async Task<ActionResult<OrderDetailResponse>> GetOrder([FromRoute] string pinCode)
    {
        var getOrderRequest = new GetOrderByPinCodeQuery(pinCode);
        return await Mediator.Send(getOrderRequest);
    }
    
    [HttpGet]
    [Authorize]
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
    [Authorize]
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
    public async Task<ActionResult<OrderItemResponse>> UpdateOrderDetail(
        [FromRoute] long id, 
        [FromRoute] long detailId,
        [FromBody] UpdateOrderDetailCommand command)
    {
        command.OrderId = id;
        command.DetailId = detailId;
        return await Mediator.Send(command);
    }
    
    [HttpPost("{id:long}/details")]
    [Authorize(Roles = "Staff")]
    public async Task<ActionResult<OrderItemResponse>> AddOrderDetail([FromRoute] long id, [FromBody] AddOrderDetailCommand command)
    {
        command.OrderId = id;
        return await Mediator.Send(command);
    }
}