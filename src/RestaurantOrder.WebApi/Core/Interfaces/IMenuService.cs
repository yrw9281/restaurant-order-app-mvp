using RestaurantOrder.WebApi.Core.DTOs;

namespace RestaurantOrder.WebApi.Core.Interfaces;

public interface IMenuService
{
    // Menu Categories
    Task<IEnumerable<MenuCategoryDto>> GetCategoriesAsync();
    Task<MenuCategoryDto?> GetCategoryByIdAsync(Guid id);
    Task<MenuCategoryDto> CreateCategoryAsync(CreateMenuCategoryDto dto);
    Task<MenuCategoryDto?> UpdateCategoryAsync(Guid id, UpdateMenuCategoryDto dto);
    Task<bool> DeleteCategoryAsync(Guid id);
    
    // Menu Items
    Task<IEnumerable<MenuItemDto>> GetMenuItemsAsync(Guid? categoryId = null, string? search = null);
    Task<MenuItemDto?> GetMenuItemByIdAsync(Guid id);
    Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto);
    Task<MenuItemDto?> UpdateMenuItemAsync(Guid id, UpdateMenuItemDto dto);
    Task<bool> DeleteMenuItemAsync(Guid id);
    
    // Menu Prices
    Task<IEnumerable<MenuPriceDto>> GetMenuPricesAsync(Guid menuItemId);
    Task<MenuPriceDto> CreateMenuPriceAsync(CreateMenuPriceDto dto);
    
    // Today's Menu (for frontend)
    Task<TodayMenuDto> GetTodayMenuAsync();
}
