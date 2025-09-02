using RestaurantOrder.WebApi.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace RestaurantOrder.WebApi.Core.DTOs;

// Order DTOs
public record OrderDto(
    Guid Id,
    string OrderNo,
    OrderType Type,
    OrderStatus Status,
    int? PartySize,
    string? TableNo,
    string? TakeoutName,
    string? TakeoutPhone,
    decimal Subtotal,
    decimal Tax,
    decimal ServiceCharge,
    decimal Total,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<OrderItemDto> Items);

public record CreateOrderDto
{
    [Required]
    public OrderType Type { get; set; }
    
    public int? PartySize { get; set; }
    
    [MaxLength(20)]
    public string? TableNo { get; set; }
    
    [MaxLength(100)]
    public string? TakeoutName { get; set; }
    
    [MaxLength(20)]
    public string? TakeoutPhone { get; set; }
}

public record UpdateOrderDto
{
    public int? PartySize { get; set; }
    
    [MaxLength(20)]
    public string? TableNo { get; set; }
    
    [MaxLength(100)]
    public string? TakeoutName { get; set; }
    
    [MaxLength(20)]
    public string? TakeoutPhone { get; set; }
}

public record OrderItemDto(
    Guid Id,
    Guid? MenuItemId,
    string SnapshotName,
    decimal SnapshotPrice,
    int Quantity,
    string? Notes);

public record CreateOrderItemDto
{
    [Required]
    public Guid MenuItemId { get; set; }
    
    [Required]
    [Range(1, 99)]
    public int Quantity { get; set; } = 1;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

public record UpdateOrderItemDto
{
    [Required]
    [Range(1, 99)]
    public int Quantity { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    PaymentMethod Method,
    DateTime PaidAt,
    Guid? ReceivedBy);

public record CreatePaymentDto
{
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public PaymentMethod Method { get; set; }
}
