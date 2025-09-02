using RestaurantOrder.WebApi.Core.Entities;

namespace RestaurantOrder.WebApi.Core.DTOs;

// Menu DTOs
public record MenuCategoryDto(Guid Id, string Name, int SortOrder, bool IsActive);

public record CreateMenuCategoryDto(string Name, int SortOrder);

public record UpdateMenuCategoryDto(string Name, int SortOrder, bool IsActive);

public record MenuItemDto(Guid Id, Guid CategoryId, string Code, string Name, bool IsActive, string CategoryName);

public record CreateMenuItemDto(Guid CategoryId, string Code, string Name);

public record UpdateMenuItemDto(string Code, string Name, bool IsActive);

public record MenuPriceDto(Guid Id, Guid MenuItemId, DateOnly EffectiveDate, decimal Price, string Currency);

public record CreateMenuPriceDto(Guid MenuItemId, DateOnly EffectiveDate, decimal Price, string Currency = "TWD");

public record MenuItemWithPriceDto(Guid Id, string Code, string Name, decimal Price, string Currency, bool IsActive);

public record MenuCategoryWithItemsDto(Guid Id, string Name, int SortOrder, IEnumerable<MenuItemWithPriceDto> Items);

public record TodayMenuDto(IEnumerable<MenuCategoryWithItemsDto> Categories);
