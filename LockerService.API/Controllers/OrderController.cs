using System.ComponentModel.DataAnnotations;
using LockerService.API.Attributes;
using LockerService.Application.Common.Enums;
using LockerService.Application.Features.Orders.Commands;
using LockerService.Application.Features.Orders.Models;
using LockerService.Application.Features.Orders.Queries;
using LockerService.Application.Features.Payments.Models;
using LockerService.Domain.Enums;

namespace LockerService.API.Controllers;

/// <summary>
/// ORDER API
/// </summary>
[ApiController]
[Route("/api/v1/orders")]
[ApiKey]
public class OrderController : ApiControllerBase
{
    /// <summary>
    /// Start new order / Reserve an order
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<OrderDetailResponse>> CreateOrder([FromBody] InitializeOrderCommand command)
    {
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Update an rder's note
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<ActionResult<OrderResponse>> UpdateOrder([FromRoute] long id, [FromBody] UpdateOrderCommand request)
    {
        request.Id = id;
        return await Mediator.Send(request);
    }
    
    /// <summary>
    /// Use order reservation at locker
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id:long}/reservation")]
    public async Task<ActionResult<OrderResponse>> UseOrderReservation([FromRoute] long id)
    {
        var command = new TakeReservationCommand()
        {
            Id = id
        };
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Confirm an order => Turn order's status to WAITING
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id:long}/confirm")]
    public async Task<ActionResult<OrderResponse>> ConfirmOrder([FromRoute] long id)
    {
        var command = new ConfirmOrderCommand()
        {
            OrderId = id,
        };

        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// [STAFF] Collect order to store => Turn order's status to COLLECTED
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id:long}/collect")]
    [Authorize]
    public async Task<ActionResult<OrderResponse>> CollectOrder([FromRoute] long id)
    {
        var command = new CollectOrderCommand()
        {
            OrderId = id,
        };
        return await Mediator.Send(command);
    }

    /// <summary>
    /// [STAFF] Mark order as processed => Turn order status to PROCESSED
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id:long}/process")]
    [Authorize]
    public async Task<ActionResult<OrderResponse>> ProcessOrder([FromRoute] long  id)
    {
        var command = new ProcessOrderCommand()
        {
            OrderId = id,
        };
        return await Mediator.Send(command);
    }

    /// <summary>
    /// [STAFF] Return processed order to locker => Turn order status to RETURNED
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id:long}/return")]
    [Authorize]
    public async Task<ActionResult<OrderResponse>> ReturnOrder([FromRoute] long id)
    {
        var command = new ReturnOrderCommand()
        {
            OrderId = id,
        };
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// [CUSTOMER] Request to checkout order => Get order checkout payment
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{id:long}/checkout")]
    public async Task<ActionResult<PaymentResponse>> CheckoutOrder([FromRoute] long id)
    {
        var command = new CheckoutOrderCommand();
        command.OrderId = id;
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Cancel order reservation
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id:long}/cancel")]
    [Authorize]
    public async Task<ActionResult<OrderResponse>> CancelReservedOrder([FromRoute] long id)
    {
        var cancelRequest = new CancelOrderCommand(id);
        return await Mediator.Send(cancelRequest);
    }
    
    /// <summary>
    /// [CUSTOMER] Open box for adding more item into locker
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id:long}/add-more")]
    public async Task<ActionResult<OrderResponse>> AddMoreItems([FromRoute] long id)
    {
        var request = new AddMoreItemsCommand(id);
        return await Mediator.Send(request);
    }


    /// <summary>
    /// [Laundromat] Process when order is overtime
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("{id:long}/process-overtime")]
    [Authorize]
    public async Task<ActionResult<OrderDetailResponse>> ProcessOvertime([FromRoute] long id)
    {
        var command = new ProcessOvertimeOrderCommand()
        {
            OrderId = id
        };
        return await Mediator.Send(command);
    }

    /// <summary>
    /// Get an order details
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Get order by pin code
    /// </summary>
    /// <param name="pinCode"></param>
    /// <param name="lockerId"></param>
    /// <returns></returns>
    [HttpGet("pin-code/{pinCode}")]
    public async Task<ActionResult<OrderDetailResponse>> GetOrder([FromRoute] string pinCode, [FromHeader] [Required] long lockerId)
    {
        var getOrderRequest = new GetOrderByPinCodeQuery(pinCode, lockerId);
        return await Mediator.Send(getOrderRequest);
    }
    
    /// <summary>
    /// Get all orders
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Get an order's order detail
    /// </summary>
    /// <param name="id"></param>
    /// <param name="detailId"></param>
    /// <returns></returns>
    [HttpGet("{id:long}/details/{detailId:long}")]
    [Authorize]
    public async Task<ActionResult<OrderItemResponse>> GetOrderDetail([FromRoute] long id, [FromRoute] long detailId)
    {
        var request = new GetOrderDetailQuery(id, detailId);
        return await Mediator.Send(request);
    }
    
    /// <summary>
    /// Remove an order's order detail
    /// </summary>
    /// <param name="id"></param>
    /// <param name="detailId"></param>
    /// <returns></returns>
    [HttpDelete("{id:long}/details/{detailId:long}")]
    [Authorize]
    public async Task<ActionResult<OrderItemResponse>> RemoveOrderDetail([FromRoute] long id, [FromRoute] long detailId)
    {
        var request = new RemoveOrderDetailCommand(id, detailId);
        return await Mediator.Send(request);
    }
    
    /// <summary>
    /// Update an order's order detail
    /// </summary>
    /// <param name="id"></param>
    /// <param name="detailId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{id:long}/details/{detailId:long}")]
    [Authorize]
    public async Task<ActionResult<OrderItemResponse>> UpdateOrderDetail(
        [FromRoute] long id, 
        [FromRoute] long detailId,
        [FromBody] UpdateOrderDetailCommand command)
    {
        command.OrderId = id;
        command.DetailId = detailId;
        return await Mediator.Send(command);
    }
    
       
    /// <summary>
    /// Get an order's order details
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:long}/details")]
    [Authorize]
    public async Task<ActionResult<ListResponse<OrderItemResponse>>> GetOrderDetails([FromRoute] long id)
    {
        var request = new GetOrderDetailsQuery(id);
        return await Mediator.Send(request);
    }
    
    /// <summary>
    /// Add an order's order details
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("{id:long}/details")]
    [Authorize]
    public async Task<ActionResult<StatusResponse>> AddOrderDetail([FromRoute] long id, [FromBody] AddOrderDetailCommand command)
    {
        command.OrderId = id;
        return await Mediator.Send(command);
    }

    /// <summary>
    /// An an order detail's laundry item
    /// </summary>
    /// <param name="id"></param>
    /// <param name="detailId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("{id:long}/details/{detailId:long}/items")]
    [Authorize]
    public async Task<ActionResult<LaundryItemResponse>> AddLaundryItem(
        [FromRoute] long id, 
        [FromRoute] long detailId,
        [FromBody] AddLaundryItemCommand command)
    {
        command.OrderId = id;
        command.OrderDetailId = detailId;
        return await Mediator.Send(command);
    }
    
    /// <summary>
    /// Remove an order detail's laundry item
    /// </summary>
    /// <param name="id"></param>
    /// <param name="detailId"></param>
    /// <param name="itemId"></param>
    /// <returns></returns>
    [HttpDelete("{id:long}/details/{detailId:long}/items/{itemId:long}")]
    [Authorize]
    public async Task<ActionResult<LaundryItemResponse>> RemoveLaundryItem(
        [FromRoute] long id, 
        [FromRoute] long detailId, 
        [FromRoute] long itemId)
    {
        var command = new RemoveLaundryItemCommand(id, detailId, itemId);
        return await Mediator.Send(command);
    }
    
    
}