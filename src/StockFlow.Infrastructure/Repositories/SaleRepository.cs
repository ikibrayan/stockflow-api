using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Domain.Entities;
using StockFlow.Infrastructure.Persistence;

namespace StockFlow.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly ApplicationDbContext _context;

    public SaleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.User)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Sale?> GetByIdAsync(int id)
    {
        return await _context.Sales
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.User)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(
        Sale sale,
        IEnumerable<InventoryMovement> inventoryMovements)
    {
        if (!_context.Database.IsRelational())
        {
            await _context.Sales.AddAsync(sale);

            await _context.InventoryMovements
                .AddRangeAsync(inventoryMovements);

            await _context.SaveChangesAsync();

            return;
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            await _context.Sales.AddAsync(sale);

            await _context.InventoryMovements
                .AddRangeAsync(inventoryMovements);

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