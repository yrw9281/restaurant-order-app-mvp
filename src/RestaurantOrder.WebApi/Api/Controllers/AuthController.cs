using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrder.WebApi.Core.DTOs;
using RestaurantOrder.WebApi.Core.Interfaces;
using System.Security.Claims;

namespace RestaurantOrder.WebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        
        if (result == null)
            return BadRequest("Registration failed. User may already exist or password requirements not met.");

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        
        if (result == null)
            return Unauthorized("Invalid email or password.");

        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return BadRequest("Invalid user ID");

        var user = await _authService.GetUserAsync(userId);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> AssignRole(Guid userId, string role)
    {
        if (!new[] { "Manager", "Server", "Cashier" }.Contains(role))
            return BadRequest("Invalid role");

        var result = await _authService.AssignRoleAsync(userId, role);
        return result ? Ok() : BadRequest("Failed to assign role");
    }

    [HttpGet("{userId}/roles")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(Guid userId)
    {
        var roles = await _authService.GetUserRolesAsync(userId);
        return Ok(roles);
    }
}
