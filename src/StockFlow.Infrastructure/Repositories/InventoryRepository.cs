using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Domain.Entities;
using StockFlow.Infrastructure.Persistence;

namespace StockFlow.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<InventoryMovement>> GetAllAsync()
    {
        return await _context.InventoryMovements
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<InventoryMovement>>
        GetByProductIdAsync(int productId)
    {
        return await _context.InventoryMovements
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.User)
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task SaveMovementAsync(
        Product product,
        InventoryMovement movement)
    {
        if (!_context.Database.IsRelational())
        {
            _context.Products.Update(product);

            await _context.InventoryMovements
                .AddAsync(movement);

            await _context.SaveChangesAsync();

            return;
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Products.Update(product);

            await _context.InventoryMovements
                .AddAsync(movement);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }
}