using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrder.WebApi.Core.DTOs;
using RestaurantOrder.WebApi.Core.Interfaces;

namespace RestaurantOrder.WebApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager,Cashier")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("daily-sales")]
    public async Task<ActionResult<IEnumerable<DailySalesDto>>> GetDailySales(
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate)
    {
        if (fromDate > toDate)
            return BadRequest("From date cannot be greater than to date");

        var sales = await _reportService.GetDailySalesAsync(fromDate, toDate);
        return Ok(sales);
    }

    [HttpGet("top-items")]
    public async Task<ActionResult<IEnumerable<TopItemDto>>> GetTopItems(
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        [FromQuery] int limit = 10)
    {
        if (fromDate > toDate)
            return BadRequest("From date cannot be greater than to date");

        if (limit <= 0 || limit > 100)
            return BadRequest("Limit must be between 1 and 100");

        var topItems = await _reportService.GetTopItemsAsync(fromDate, toDate, limit);
        return Ok(topItems);
    }
}
