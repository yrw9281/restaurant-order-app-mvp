using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantOrder.WebApi.Core.DTOs;
using RestaurantOrder.WebApi.Core.Identity;
using RestaurantOrder.WebApi.Core.Interfaces;
using RestaurantOrder.WebApi.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace RestaurantOrder.WebApi.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<AppRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return null; // User already exists

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            DisplayName = dto.DisplayName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return null;

        // Assign default Server role
        await _userManager.AddToRoleAsync(user, "Server");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return new AuthResponseDto(user.Id, user.Email!, user.DisplayName, roles, token);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return new AuthResponseDto(user.Id, user.Email!, user.DisplayName, roles, token);
    }

    public async Task<UserDto?> GetUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto(user.Id, user.Email!, user.DisplayName, roles);
    }

    public async Task<bool> AssignRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Enumerable.Empty<string>();

        return await _userManager.GetRolesAsync(user);
    }

    private string GenerateJwtToken(AppUser user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? "your-secret-key-here-must-be-at-least-256-bits"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.DisplayName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "restaurant-order-api",
            audience: _configuration["Jwt:Audience"] ?? "restaurant-order-client",
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class AuditService : IAuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string action, string entity, Guid? entityId, object? summary = null, Guid? userId = null)
    {
        var auditLog = new Core.Entities.AuditLog
        {
            UserId = userId,
            Action = action,
            Entity = entity,
            EntityId = entityId,
            Summary = summary != null ? JsonSerializer.Serialize(summary) : null
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }
}

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DailySalesDto>> GetDailySalesAsync(DateOnly fromDate, DateOnly toDate)
    {
        return await _context.Payments
            .Where(p => DateOnly.FromDateTime(p.PaidAt) >= fromDate && DateOnly.FromDateTime(p.PaidAt) <= toDate)
            .GroupBy(p => DateOnly.FromDateTime(p.PaidAt))
            .Select(g => new DailySalesDto(
                g.Key,
                g.Sum(p => p.Amount),
                g.Select(p => p.OrderId).Distinct().Count()
            ))
            .OrderBy(d => d.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<TopItemDto>> GetTopItemsAsync(DateOnly fromDate, DateOnly toDate, int limit = 10)
    {
        return await _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => DateOnly.FromDateTime(oi.Order.CreatedAt) >= fromDate && 
                        DateOnly.FromDateTime(oi.Order.CreatedAt) <= toDate &&
                        oi.Order.Status == Core.Entities.OrderStatus.Paid)
            .GroupBy(oi => oi.SnapshotName)
            .Select(g => new TopItemDto(
                g.Key,
                g.Sum(oi => oi.Quantity),
                g.Sum(oi => oi.SnapshotPrice * oi.Quantity)
            ))
            .OrderByDescending(t => t.TotalQuantity)
            .Take(limit)
            .ToListAsync();
    }
}
