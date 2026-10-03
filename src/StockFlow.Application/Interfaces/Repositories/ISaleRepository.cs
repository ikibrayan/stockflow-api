using StockFlow.Domain.Entities;

namespace StockFlow.Application.Interfaces.Repositories;

public interface ISaleRepository
{
    Task<List<Sale>> GetAllAsync();

    Task<Sale?> GetByIdAsync(int id);

    Task AddAsync(
        Sale sale,
        IEnumerable<InventoryMovement> inventoryMovements);
}