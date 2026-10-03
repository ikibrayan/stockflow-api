using StockFlow.Application.DTOs.Inventory;

namespace StockFlow.Application.Interfaces.Services;

public interface IInventoryService
{
    Task<List<InventoryMovementResponseDto>> GetAllAsync();

    Task<List<InventoryMovementResponseDto>> GetByProductIdAsync(
        int productId);

    Task<InventoryMovementResponseDto> CreateEntryAsync(
        CreateInventoryEntryDto dto,
        int userId);

    Task<InventoryMovementResponseDto> CreateAdjustmentAsync(
        CreateInventoryAdjustmentDto dto,
        int userId);
}