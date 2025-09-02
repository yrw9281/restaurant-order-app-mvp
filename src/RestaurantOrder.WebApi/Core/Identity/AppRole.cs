using Microsoft.AspNetCore.Identity;

namespace RestaurantOrder.WebApi.Core.Identity;

public class AppRole : IdentityRole<Guid>
{
    public AppRole() : base() { }
    
    public AppRole(string roleName) : base(roleName) { }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
