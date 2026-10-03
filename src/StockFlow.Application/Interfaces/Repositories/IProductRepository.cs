using StockFlow.Domain.Entities;

namespace StockFlow.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<Product?> GetBySkuAsync(string sku);

    Task AddAsync(Product product);

    Task UpdateAsync(Product product);

    Task<bool> ExistsBySkuAsync(string sku);
}