using Microsoft.EntityFrameworkCore;
using RestaurantOrder.WebApi.Core.DTOs;
using RestaurantOrder.WebApi.Core.Entities;
using RestaurantOrder.WebApi.Core.Interfaces;
using RestaurantOrder.WebApi.Infrastructure;

namespace RestaurantOrder.WebApi.Infrastructure.Services;

public class MenuService : IMenuService
{
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;

    public MenuService(AppDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    // Menu Categories
    public async Task<IEnumerable<MenuCategoryDto>> GetCategoriesAsync()
    {
        return await _context.MenuCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new MenuCategoryDto(c.Id, c.Name, c.SortOrder, c.IsActive))
            .ToListAsync();
    }

    public async Task<MenuCategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var category = await _context.MenuCategories
            .Where(c => c.Id == id)
            .Select(c => new MenuCategoryDto(c.Id, c.Name, c.SortOrder, c.IsActive))
            .FirstOrDefaultAsync();
        
        return category;
    }

    public async Task<MenuCategoryDto> CreateCategoryAsync(CreateMenuCategoryDto dto)
    {
        var category = new MenuCategory
        {
            Name = dto.Name,
            SortOrder = dto.SortOrder
        };

        _context.MenuCategories.Add(category);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Create", "MenuCategory", category.Id, new { category.Name, category.SortOrder });

        return new MenuCategoryDto(category.Id, category.Name, category.SortOrder, category.IsActive);
    }

    public async Task<MenuCategoryDto?> UpdateCategoryAsync(Guid id, UpdateMenuCategoryDto dto)
    {
        var category = await _context.MenuCategories.FindAsync(id);
        if (category == null) return null;

        category.Name = dto.Name;
        category.SortOrder = dto.SortOrder;
        category.IsActive = dto.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Update", "MenuCategory", category.Id, new { category.Name, category.SortOrder, category.IsActive });

        return new MenuCategoryDto(category.Id, category.Name, category.SortOrder, category.IsActive);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var category = await _context.MenuCategories.FindAsync(id);
        if (category == null) return false;

        // Soft delete
        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Delete", "MenuCategory", category.Id, new { category.Name });

        return true;
    }

    // Menu Items
    public async Task<IEnumerable<MenuItemDto>> GetMenuItemsAsync(Guid? categoryId = null, string? search = null)
    {
        var query = _context.MenuItems
            .Include(m => m.Category)
            .Where(m => m.IsActive);

        if (categoryId.HasValue)
            query = query.Where(m => m.CategoryId == categoryId.Value);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(m => m.Name.Contains(search) || m.Code.Contains(search));

        return await query
            .Select(m => new MenuItemDto(m.Id, m.CategoryId, m.Code, m.Name, m.IsActive, m.Category.Name))
            .ToListAsync();
    }

    public async Task<MenuItemDto?> GetMenuItemByIdAsync(Guid id)
    {
        return await _context.MenuItems
            .Include(m => m.Category)
            .Where(m => m.Id == id)
            .Select(m => new MenuItemDto(m.Id, m.CategoryId, m.Code, m.Name, m.IsActive, m.Category.Name))
            .FirstOrDefaultAsync();
    }

    public async Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto)
    {
        var menuItem = new MenuItem
        {
            CategoryId = dto.CategoryId,
            Code = dto.Code,
            Name = dto.Name
        };

        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        // Load category name for response
        var category = await _context.MenuCategories.FindAsync(dto.CategoryId);

        await _auditService.LogAsync("Create", "MenuItem", menuItem.Id, new { menuItem.Code, menuItem.Name, menuItem.CategoryId });

        return new MenuItemDto(menuItem.Id, menuItem.CategoryId, menuItem.Code, menuItem.Name, menuItem.IsActive, category?.Name ?? "");
    }

    public async Task<MenuItemDto?> UpdateMenuItemAsync(Guid id, UpdateMenuItemDto dto)
    {
        var menuItem = await _context.MenuItems.Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == id);
        if (menuItem == null) return null;

        menuItem.Code = dto.Code;
        menuItem.Name = dto.Name;
        menuItem.IsActive = dto.IsActive;
        menuItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Update", "MenuItem", menuItem.Id, new { menuItem.Code, menuItem.Name, menuItem.IsActive });

        return new MenuItemDto(menuItem.Id, menuItem.CategoryId, menuItem.Code, menuItem.Name, menuItem.IsActive, menuItem.Category.Name);
    }

    public async Task<bool> DeleteMenuItemAsync(Guid id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null) return false;

        menuItem.IsActive = false;
        menuItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Delete", "MenuItem", menuItem.Id, new { menuItem.Code, menuItem.Name });

        return true;
    }

    // Menu Prices
    public async Task<IEnumerable<MenuPriceDto>> GetMenuPricesAsync(Guid menuItemId)
    {
        return await _context.MenuPrices
            .Where(p => p.MenuItemId == menuItemId)
            .OrderByDescending(p => p.EffectiveDate)
            .Select(p => new MenuPriceDto(p.Id, p.MenuItemId, p.EffectiveDate, p.Price, p.Currency))
            .ToListAsync();
    }

    public async Task<MenuPriceDto> CreateMenuPriceAsync(CreateMenuPriceDto dto)
    {
        var menuPrice = new MenuPrice
        {
            MenuItemId = dto.MenuItemId,
            EffectiveDate = dto.EffectiveDate,
            Price = dto.Price,
            Currency = dto.Currency
        };

        _context.MenuPrices.Add(menuPrice);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("Create", "MenuPrice", menuPrice.Id, new { menuPrice.MenuItemId, menuPrice.EffectiveDate, menuPrice.Price });

        return new MenuPriceDto(menuPrice.Id, menuPrice.MenuItemId, menuPrice.EffectiveDate, menuPrice.Price, menuPrice.Currency);
    }

    // Today's Menu
    public async Task<TodayMenuDto> GetTodayMenuAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var categories = await _context.MenuCategories
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Include(c => c.MenuItems.Where(m => m.IsActive))
            .ThenInclude(m => m.MenuPrices.Where(p => p.EffectiveDate == today))
            .ToListAsync();

        var categoryDtos = categories.Select(c => new MenuCategoryWithItemsDto(
            c.Id,
            c.Name,
            c.SortOrder,
            c.MenuItems.Where(m => m.MenuPrices.Any()).Select(m =>
            {
                var price = m.MenuPrices.First();
                return new MenuItemWithPriceDto(m.Id, m.Code, m.Name, price.Price, price.Currency, m.IsActive);
            })
        ));

        return new TodayMenuDto(categoryDtos);
    }
}
