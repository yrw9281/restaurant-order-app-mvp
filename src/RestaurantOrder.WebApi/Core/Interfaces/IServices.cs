using RestaurantOrder.WebApi.Core.DTOs;

namespace RestaurantOrder.WebApi.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<UserDto?> GetUserAsync(Guid userId);
    Task<bool> AssignRoleAsync(Guid userId, string role);
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
}

public interface IAuditService
{
    Task LogAsync(string action, string entity, Guid? entityId, object? summary = null, Guid? userId = null);
}

public interface IReportService
{
    Task<IEnumerable<DailySalesDto>> GetDailySalesAsync(DateOnly fromDate, DateOnly toDate);
    Task<IEnumerable<TopItemDto>> GetTopItemsAsync(DateOnly fromDate, DateOnly toDate, int limit = 10);
}
