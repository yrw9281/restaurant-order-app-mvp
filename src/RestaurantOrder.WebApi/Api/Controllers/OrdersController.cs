using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrder.WebApi.Core.DTOs;
using RestaurantOrder.WebApi.Core.Entities;
using RestaurantOrder.WebApi.Core.Interfaces;
using System.Security.Claims;

namespace RestaurantOrder.WebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    #region Orders

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(
        [FromQuery] OrderStatus? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var orders = await _orderService.GetOrdersAsync(status, fromDate, toDate);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        return order == null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.CreateOrderAsync(dto, userId);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDto>> UpdateOrder(Guid id, UpdateOrderDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.UpdateOrderAsync(id, dto, userId);
            return order == null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _orderService.DeleteOrderAsync(id, userId);
            return result ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region Order Items

    [HttpPost("{id}/items")]
    public async Task<ActionResult<OrderDto>> AddOrderItem(Guid id, CreateOrderItemDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.AddOrderItemAsync(id, dto, userId);
            return order == null ? NotFound() : Ok(order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/items/{itemId}")]
    public async Task<ActionResult<OrderDto>> UpdateOrderItem(Guid id, Guid itemId, UpdateOrderItemDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.UpdateOrderItemAsync(id, itemId, dto, userId);
            return order == null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}/items/{itemId}")]
    public async Task<ActionResult<OrderDto>> RemoveOrderItem(Guid id, Guid itemId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.RemoveOrderItemAsync(id, itemId, userId);
            return order == null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region Order Status Flow

    [HttpPost("{id}/submit")]
    public async Task<ActionResult<OrderDto>> SubmitOrder(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.SubmitOrderAsync(id, userId);
            return order == null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/confirm")]
    [Authorize(Roles = "Manager,Cashier")]
    public async Task<ActionResult<OrderDto>> ConfirmOrder(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.ConfirmOrderAsync(id, userId);
            return order == null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Manager,Cashier")]
    public async Task<ActionResult<OrderDto>> CancelOrder(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.CancelOrderAsync(id, userId);
            return order == null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region Payments

    [HttpPost("{id}/pay")]
    [Authorize(Roles = "Manager,Cashier")]
    public async Task<ActionResult<OrderDto>> PayOrder(Guid id, CreatePaymentDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderService.PayOrderAsync(id, dto, userId);
            return order == null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/payments")]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetOrderPayments(Guid id)
    {
        var payments = await _orderService.GetOrderPaymentsAsync(id);
        return Ok(payments);
    }

    #endregion
}
