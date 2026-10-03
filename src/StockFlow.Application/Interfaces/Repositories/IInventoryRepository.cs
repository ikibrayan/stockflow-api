using StockFlow.Domain.Entities;

namespace StockFlow.Application.Interfaces.Repositories;

public interface IInventoryRepository
{
    Task<List<InventoryMovement>> GetAllAsync();

    Task<List<InventoryMovement>> GetByProductIdAsync(
        int productId);

    Task SaveMovementAsync(
        Product product,
        InventoryMovement movement);
}