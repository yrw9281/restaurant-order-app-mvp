using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantOrder.WebApi.Core.Entities;

public enum OrderType
{
    DineIn,
    Takeout
}

public enum OrderStatus
{
    Draft,
    Submitted,
    Confirmed,
    Paid,
    Cancelled
}

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(50)]
    public string OrderNo { get; set; } = string.Empty;
    
    [Required]
    public OrderType Type { get; set; }
    
    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    
    public int? PartySize { get; set; }
    
    [MaxLength(20)]
    public string? TableNo { get; set; }
    
    [MaxLength(100)]
    public string? TakeoutName { get; set; }
    
    [MaxLength(20)]
    public string? TakeoutPhone { get; set; }
    
    [Column(TypeName = "decimal(12,2)")]
    public decimal Subtotal { get; set; }
    
    [Column(TypeName = "decimal(12,2)")]
    public decimal Tax { get; set; }
    
    [Column(TypeName = "decimal(12,2)")]
    public decimal ServiceCharge { get; set; }
    
    [Column(TypeName = "decimal(12,2)")]
    public decimal Total { get; set; }
    
    public Guid? CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Timestamp]
    public byte[]? RowVersion { get; set; }
    
    // Navigation properties
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
