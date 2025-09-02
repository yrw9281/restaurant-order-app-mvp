using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantOrder.WebApi.Core.Entities;

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid OrderId { get; set; }
    
    public Guid? MenuItemId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string SnapshotName { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(12,2)")]
    public decimal SnapshotPrice { get; set; }
    
    [Required]
    public int Quantity { get; set; } = 1;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; } = null!;
    
    [ForeignKey("MenuItemId")]
    public virtual MenuItem? MenuItem { get; set; }
}
