using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.DTOs.Inventory;
using StockFlow.Application.Interfaces.Services;

namespace StockFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("movements")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllMovements()
    {
        var movements = await _inventoryService.GetAllAsync();

        return Ok(movements);
    }

    [HttpGet("movements/product/{productId:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetMovementsByProduct(
        int productId)
    {
        var movements =
            await _inventoryService.GetByProductIdAsync(productId);

        return Ok(movements);
    }

    [HttpPost("entry")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateEntry(
        CreateInventoryEntryDto dto)
    {
        var userId = GetUserId();

        var movement =
            await _inventoryService.CreateEntryAsync(
                dto,
                userId);

        return Ok(movement);
    }

    [HttpPost("adjustment")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateAdjustment(
        CreateInventoryAdjustmentDto dto)
    {
        var userId = GetUserId();

        var movement =
            await _inventoryService.CreateAdjustmentAsync(
                dto,
                userId);

        return Ok(movement);
    }

    private int GetUserId()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdClaim) ||
            !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException(
                "The authenticated user could not be identified.");
        }

        return userId;
    }
}