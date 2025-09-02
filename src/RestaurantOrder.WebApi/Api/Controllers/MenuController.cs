using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrder.WebApi.Core.DTOs;
using RestaurantOrder.WebApi.Core.Interfaces;

namespace RestaurantOrder.WebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    // Public endpoint for today's menu
    [HttpGet("today")]
    [AllowAnonymous]
    public async Task<ActionResult<TodayMenuDto>> GetTodayMenu()
    {
        var menu = await _menuService.GetTodayMenuAsync();
        return Ok(menu);
    }

    #region Menu Categories

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<MenuCategoryDto>>> GetCategories()
    {
        var categories = await _menuService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("categories/{id}")]
    public async Task<ActionResult<MenuCategoryDto>> GetCategory(Guid id)
    {
        var category = await _menuService.GetCategoryByIdAsync(id);
        return category == null ? NotFound() : Ok(category);
    }

    [HttpPost("categories")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<MenuCategoryDto>> CreateCategory(CreateMenuCategoryDto dto)
    {
        var category = await _menuService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpPut("categories/{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<MenuCategoryDto>> UpdateCategory(Guid id, UpdateMenuCategoryDto dto)
    {
        var category = await _menuService.UpdateCategoryAsync(id, dto);
        return category == null ? NotFound() : Ok(category);
    }

    [HttpDelete("categories/{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _menuService.DeleteCategoryAsync(id);
        return result ? NoContent() : NotFound();
    }

    #endregion

    #region Menu Items

    [HttpGet("items")]
    public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuItems(
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? search = null)
    {
        var items = await _menuService.GetMenuItemsAsync(categoryId, search);
        return Ok(items);
    }

    [HttpGet("items/{id}")]
    public async Task<ActionResult<MenuItemDto>> GetMenuItem(Guid id)
    {
        var item = await _menuService.GetMenuItemByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost("items")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<MenuItemDto>> CreateMenuItem(CreateMenuItemDto dto)
    {
        try
        {
            var item = await _menuService.CreateMenuItemAsync(dto);
            return CreatedAtAction(nameof(GetMenuItem), new { id = item.Id }, item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("items/{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<MenuItemDto>> UpdateMenuItem(Guid id, UpdateMenuItemDto dto)
    {
        var item = await _menuService.UpdateMenuItemAsync(id, dto);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpDelete("items/{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteMenuItem(Guid id)
    {
        var result = await _menuService.DeleteMenuItemAsync(id);
        return result ? NoContent() : NotFound();
    }

    #endregion

    #region Menu Prices

    [HttpGet("items/{itemId}/prices")]
    public async Task<ActionResult<IEnumerable<MenuPriceDto>>> GetMenuPrices(Guid itemId)
    {
        var prices = await _menuService.GetMenuPricesAsync(itemId);
        return Ok(prices);
    }

    [HttpPost("items/{itemId}/prices")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<MenuPriceDto>> CreateMenuPrice(Guid itemId, CreateMenuPriceDto dto)
    {
        if (dto.MenuItemId != itemId)
            return BadRequest("Menu item ID mismatch");

        try
        {
            var price = await _menuService.CreateMenuPriceAsync(dto);
            return CreatedAtAction(nameof(GetMenuPrices), new { itemId }, price);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    #endregion
}
