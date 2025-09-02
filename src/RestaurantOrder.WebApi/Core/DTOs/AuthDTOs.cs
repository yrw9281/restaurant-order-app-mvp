using System.ComponentModel.DataAnnotations;

namespace RestaurantOrder.WebApi.Core.DTOs;

// Auth DTOs
public record RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;
}

public record LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public record AuthResponseDto(
    Guid UserId,
    string Email,
    string DisplayName,
    IEnumerable<string> Roles,
    string Token);

public record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    IEnumerable<string> Roles);

// Reports DTOs
public record DailySalesDto(DateOnly Date, decimal TotalAmount, int OrdersCount);

public record TopItemDto(string ItemName, int TotalQuantity, decimal TotalRevenue);
