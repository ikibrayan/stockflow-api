using StockFlow.Application.DTOs.Customers;

namespace StockFlow.Application.Interfaces.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseDto>> GetAllAsync();

    Task<CustomerResponseDto?> GetByIdAsync(int id);

    Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto);

    Task<bool> UpdateAsync(int id, UpdateCustomerDto dto);
}