using Microsoft.EntityFrameworkCore;
using RestaurantOrder.WebApi.Core.DTOs;
using RestaurantOrder.WebApi.Core.Entities;
using RestaurantOrder.WebApi.Core.Interfaces;
using RestaurantOrder.WebApi.Infrastructure;

namespace RestaurantOrder.WebApi.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;

    public OrderService(AppDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    // Orders
    public async Task<IEnumerable<OrderDto>> GetOrdersAsync(OrderStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (fromDate.HasValue)
            query = query.Where(o => o.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(o => o.CreatedAt <= toDate.Value);

        return await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => MapToOrderDto(o))
            .ToListAsync();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order == null ? null : MapToOrderDto(order);
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, Guid userId)
    {
        // Validate business rules
        if (dto.Type == OrderType.DineIn && (string.IsNullOrEmpty(dto.TableNo) || !dto.PartySize.HasValue))
            throw new ArgumentException("DineIn orders require TableNo and PartySize");

        var order = new Order
        {
            OrderNo = await GenerateOrderNoAsync(),
            Type = dto.Type,
            PartySize = dto.PartySize,
            TableNo = dto.TableNo,
            TakeoutName = dto.TakeoutName,
            TakeoutPhone = dto.TakeoutPhone,
            CreatedBy = userId
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Create", "Order", order.Id, new { order.OrderNo, order.Type }, userId);

        return MapToOrderDto(order);
    }

    public async Task<OrderDto?> UpdateOrderAsync(Guid id, UpdateOrderDto dto, Guid userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        // Only allow updates on Draft or Submitted orders
        if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.Submitted)
            throw new InvalidOperationException("Order cannot be modified in current status");

        order.PartySize = dto.PartySize;
        order.TableNo = dto.TableNo;
        order.TakeoutName = dto.TakeoutName;
        order.TakeoutPhone = dto.TakeoutPhone;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Update", "Order", order.Id, new { order.PartySize, order.TableNo, order.TakeoutName, order.TakeoutPhone }, userId);

        return MapToOrderDto(order);
    }

    public async Task<bool> DeleteOrderAsync(Guid id, Guid userId)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        if (order.Status != OrderStatus.Draft)
            throw new InvalidOperationException("Only draft orders can be deleted");

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Delete", "Order", order.Id, new { order.OrderNo }, userId);

        return true;
    }

    // Order Items
    public async Task<OrderDto?> AddOrderItemAsync(Guid orderId, CreateOrderItemDto dto, Guid userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return null;

        if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.Submitted)
            throw new InvalidOperationException("Order cannot be modified in current status");

        // Get today's price for the menu item
        var today = DateOnly.FromDateTime(DateTime.Today);
        var menuPrice = await _context.MenuPrices
            .Include(p => p.MenuItem)
            .FirstOrDefaultAsync(p => p.MenuItemId == dto.MenuItemId && p.EffectiveDate == today);

        if (menuPrice == null)
            throw new ArgumentException("No price found for this menu item today");

        var orderItem = new OrderItem
        {
            OrderId = orderId,
            MenuItemId = dto.MenuItemId,
            SnapshotName = menuPrice.MenuItem.Name,
            SnapshotPrice = menuPrice.Price,
            Quantity = dto.Quantity,
            Notes = dto.Notes
        };

        _context.OrderItems.Add(orderItem);

        // Recalculate totals
        await RecalculateOrderTotalsAsync(order);
        
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("AddItem", "Order", order.Id, new { dto.MenuItemId, dto.Quantity, orderItem.SnapshotPrice }, userId);

        return MapToOrderDto(order);
    }

    public async Task<OrderDto?> UpdateOrderItemAsync(Guid orderId, Guid itemId, UpdateOrderItemDto dto, Guid userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return null;

        if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.Submitted)
            throw new InvalidOperationException("Order cannot be modified in current status");

        var orderItem = order.OrderItems.FirstOrDefault(i => i.Id == itemId);
        if (orderItem == null) return null;

        orderItem.Quantity = dto.Quantity;
        orderItem.Notes = dto.Notes;
        orderItem.UpdatedAt = DateTime.UtcNow;

        await RecalculateOrderTotalsAsync(order);
        
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("UpdateItem", "Order", order.Id, new { itemId, dto.Quantity }, userId);

        return MapToOrderDto(order);
    }

    public async Task<OrderDto?> RemoveOrderItemAsync(Guid orderId, Guid itemId, Guid userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return null;

        if (order.Status != OrderStatus.Draft && order.Status != OrderStatus.Submitted)
            throw new InvalidOperationException("Order cannot be modified in current status");

        var orderItem = order.OrderItems.FirstOrDefault(i => i.Id == itemId);
        if (orderItem == null) return null;

        _context.OrderItems.Remove(orderItem);

        await RecalculateOrderTotalsAsync(order);
        
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("RemoveItem", "Order", order.Id, new { itemId }, userId);

        return MapToOrderDto(order);
    }

    // Order Status Flow
    public async Task<OrderDto?> SubmitOrderAsync(Guid id, Guid userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        if (order.Status != OrderStatus.Draft)
            throw new InvalidOperationException("Only draft orders can be submitted");

        if (!order.OrderItems.Any())
            throw new InvalidOperationException("Cannot submit empty order");

        order.Status = OrderStatus.Submitted;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Submit", "Order", order.Id, new { order.OrderNo }, userId);

        return MapToOrderDto(order);
    }

    public async Task<OrderDto?> ConfirmOrderAsync(Guid id, Guid userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        if (order.Status != OrderStatus.Submitted)
            throw new InvalidOperationException("Only submitted orders can be confirmed");

        order.Status = OrderStatus.Confirmed;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Confirm", "Order", order.Id, new { order.OrderNo }, userId);

        return MapToOrderDto(order);
    }

    public async Task<OrderDto?> CancelOrderAsync(Guid id, Guid userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        if (order.Status == OrderStatus.Paid)
            throw new InvalidOperationException("Paid orders cannot be cancelled");

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Cancel", "Order", order.Id, new { order.OrderNo }, userId);

        return MapToOrderDto(order);
    }

    // Payment
    public async Task<OrderDto?> PayOrderAsync(Guid id, CreatePaymentDto dto, Guid userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        if (order.Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be paid");

        var payment = new Payment
        {
            OrderId = id,
            Amount = dto.Amount,
            Method = dto.Method,
            ReceivedBy = userId
        };

        _context.Payments.Add(payment);

        // Check if fully paid
        var totalPaid = order.Payments.Sum(p => p.Amount) + dto.Amount;
        if (totalPaid >= order.Total)
        {
            order.Status = OrderStatus.Paid;
            order.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Pay", "Order", order.Id, new { order.OrderNo, dto.Amount, dto.Method }, userId);

        return MapToOrderDto(order);
    }

    public async Task<IEnumerable<PaymentDto>> GetOrderPaymentsAsync(Guid orderId)
    {
        return await _context.Payments
            .Where(p => p.OrderId == orderId)
            .Select(p => new PaymentDto(p.Id, p.OrderId, p.Amount, p.Method, p.PaidAt, p.ReceivedBy))
            .ToListAsync();
    }

    // Helper methods
    private async Task<string> GenerateOrderNoAsync()
    {
        var date = DateTime.Today.ToString("yyyyMMdd");
        var count = await _context.Orders.CountAsync(o => o.CreatedAt.Date == DateTime.Today) + 1;
        return $"{date}-{count:D4}";
    }

    private async Task RecalculateOrderTotalsAsync(Order order)
    {
        await _context.Entry(order).Collection(o => o.OrderItems).LoadAsync();
        
        order.Subtotal = order.OrderItems.Sum(i => i.SnapshotPrice * i.Quantity);
        order.Tax = order.Subtotal * 0.05m; // 5% tax
        order.ServiceCharge = order.Type == OrderType.DineIn ? order.Subtotal * 0.10m : 0; // 10% service charge for dine-in
        order.Total = order.Subtotal + order.Tax + order.ServiceCharge;
        order.UpdatedAt = DateTime.UtcNow;
    }

    private static OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto(
            order.Id,
            order.OrderNo,
            order.Type,
            order.Status,
            order.PartySize,
            order.TableNo,
            order.TakeoutName,
            order.TakeoutPhone,
            order.Subtotal,
            order.Tax,
            order.ServiceCharge,
            order.Total,
            order.CreatedAt,
            order.UpdatedAt,
            order.OrderItems.Select(i => new OrderItemDto(i.Id, i.MenuItemId, i.SnapshotName, i.SnapshotPrice, i.Quantity, i.Notes))
        );
    }
}
