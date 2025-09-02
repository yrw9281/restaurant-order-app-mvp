using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantOrder.WebApi.Core.Entities;

public class MenuPrice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid MenuItemId { get; set; }
    
    [Required]
    public DateOnly EffectiveDate { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(12,2)")]
    public decimal Price { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = "TWD";
    
    public Guid? CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("MenuItemId")]
    public virtual MenuItem MenuItem { get; set; } = null!;
}
