using RestaurantOrder.WebApi.Core.DTOs;
using RestaurantOrder.WebApi.Core.Entities;

namespace RestaurantOrder.WebApi.Core.Interfaces;

public interface IOrderService
{
    // Orders
    Task<IEnumerable<OrderDto>> GetOrdersAsync(OrderStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<OrderDto?> GetOrderByIdAsync(Guid id);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, Guid userId);
    Task<OrderDto?> UpdateOrderAsync(Guid id, UpdateOrderDto dto, Guid userId);
    Task<bool> DeleteOrderAsync(Guid id, Guid userId);
    
    // Order Items
    Task<OrderDto?> AddOrderItemAsync(Guid orderId, CreateOrderItemDto dto, Guid userId);
    Task<OrderDto?> UpdateOrderItemAsync(Guid orderId, Guid itemId, UpdateOrderItemDto dto, Guid userId);
    Task<OrderDto?> RemoveOrderItemAsync(Guid orderId, Guid itemId, Guid userId);
    
    // Order Status Flow
    Task<OrderDto?> SubmitOrderAsync(Guid id, Guid userId);
    Task<OrderDto?> ConfirmOrderAsync(Guid id, Guid userId);
    Task<OrderDto?> CancelOrderAsync(Guid id, Guid userId);
    
    // Payment
    Task<OrderDto?> PayOrderAsync(Guid id, CreatePaymentDto dto, Guid userId);
    Task<IEnumerable<PaymentDto>> GetOrderPaymentsAsync(Guid orderId);
}
