using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RestaurantOrder.WebApi.Core.Identity;

public class AppUser : IdentityUser<Guid>
{
    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
