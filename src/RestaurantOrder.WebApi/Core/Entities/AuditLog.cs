using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantOrder.WebApi.Core.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid? UserId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Entity { get; set; } = string.Empty;
    
    public Guid? EntityId { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string? Summary { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
