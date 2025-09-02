using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantOrder.WebApi.Core.Entities;

public class MenuItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid CategoryId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("CategoryId")]
    public virtual MenuCategory Category { get; set; } = null!;
    
    public virtual ICollection<MenuPrice> MenuPrices { get; set; } = new List<MenuPrice>();
    
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
