using StockFlow.Domain.Entities;

namespace StockFlow.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();

    Task<Customer?> GetByIdAsync(int id);

    Task<Customer?> GetByDocumentAsync(string document);

    Task<bool> ExistsByDocumentAsync(string document);

    Task AddAsync(Customer customer);

    Task UpdateAsync(Customer customer);
}