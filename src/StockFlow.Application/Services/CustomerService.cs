using StockFlow.Application.DTOs.Customers;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Application.Interfaces.Services;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerResponseDto>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();

        return customers.Select(MapToResponse);
    }

    public async Task<CustomerResponseDto?> GetByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer is null)
            return null;

        return MapToResponse(customer);
    }

    public async Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto)
    {
        var documentExists =
            await _customerRepository.ExistsByDocumentAsync(dto.Document);

        if (documentExists)
            throw new InvalidOperationException(
                "A customer with this document already exists.");

        var customer = new Customer
        {
            Name = dto.Name,
            Document = dto.Document,
            Email = dto.Email,
            Phone = dto.Phone
        };

        await _customerRepository.AddAsync(customer);

        return MapToResponse(customer);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCustomerDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer is null)
            return false;

        if (!string.Equals(
                customer.Document,
                dto.Document,
                StringComparison.OrdinalIgnoreCase))
        {
            var documentExists =
                await _customerRepository.ExistsByDocumentAsync(dto.Document);

            if (documentExists)
                throw new InvalidOperationException(
                    "A customer with this document already exists.");
        }

        customer.Name = dto.Name;
        customer.Document = dto.Document;
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;

        await _customerRepository.UpdateAsync(customer);

        return true;
    }

    private static CustomerResponseDto MapToResponse(Customer customer)
    {
        return new CustomerResponseDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Document = customer.Document,
            Email = customer.Email,
            Phone = customer.Phone,
            CreatedAt = customer.CreatedAt
        };
    }
}