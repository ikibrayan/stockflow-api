using StockFlow.Application.DTOs.Sales;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Application.Interfaces.Services;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;

    public SaleService(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IUserRepository userRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _userRepository = userRepository;
    }

    public async Task<List<SaleResponseDto>> GetAllAsync()
    {
        var sales = await _saleRepository.GetAllAsync();

        return sales
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<SaleResponseDto?> GetByIdAsync(int id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);

        if (sale is null)
            return null;

        return MapToResponse(sale);
    }

    public async Task<SaleResponseDto> CreateAsync(
        CreateSaleDto dto,
        int userId)
    {
        var customer =
            await _customerRepository.GetByIdAsync(dto.CustomerId);

        if (customer is null)
        {
            throw new KeyNotFoundException(
                "Customer not found.");
        }

        var user =
            await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        var groupedItems = dto.Items
            .GroupBy(x => x.ProductId)
            .Select(x => new
            {
                ProductId = x.Key,
                Quantity = x.Sum(i => i.Quantity)
            })
            .ToList();

        var sale = new Sale
        {
            CustomerId = dto.CustomerId,
            UserId = userId,
            Total = 0
        };

        var inventoryMovements =
            new List<InventoryMovement>();

        decimal total = 0;

        foreach (var item in groupedItems)
        {
            var product =
                await _productRepository.GetByIdAsync(
                    item.ProductId);

            if (product is null)
            {
                throw new KeyNotFoundException(
                    $"Product with id {item.ProductId} was not found.");
            }

            if (!product.IsActive)
            {
                throw new InvalidOperationException(
                    $"Product '{product.Name}' is inactive.");
            }

            if (product.Stock < item.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for product '{product.Name}'. " +
                    $"Available: {product.Stock}, requested: {item.Quantity}.");
            }

            var previousStock = product.Stock;
            var newStock = previousStock - item.Quantity;

            var subtotal =
                product.Price * item.Quantity;

            product.Stock = newStock;

            sale.Items.Add(new SaleItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                Subtotal = subtotal
            });

            inventoryMovements.Add(
                new InventoryMovement
                {
                    ProductId = product.Id,
                    UserId = userId,
                    Type = InventoryMovementType.Exit,
                    Quantity = item.Quantity,
                    PreviousStock = previousStock,
                    NewStock = newStock,
                    Reference = "Sale"
                });

            total += subtotal;
        }

        sale.Total = total;

        await _saleRepository.AddAsync(
            sale,
            inventoryMovements);

        var createdSale =
            await _saleRepository.GetByIdAsync(sale.Id);

        if (createdSale is null)
        {
            throw new InvalidOperationException(
                "The sale could not be retrieved after creation.");
        }

        return MapToResponse(createdSale);
    }

    private static SaleResponseDto MapToResponse(
        Sale sale)
    {
        return new SaleResponseDto
        {
            Id = sale.Id,
            CustomerId = sale.CustomerId,
            CustomerName =
                sale.Customer?.Name ?? string.Empty,

            UserId = sale.UserId,
            UserName =
                sale.User?.Name ?? string.Empty,

            Total = sale.Total,
            CreatedAt = sale.CreatedAt,

            Items = sale.Items
                .Select(item =>
                    new SaleItemResponseDto
                    {
                        ProductId = item.ProductId,
                        ProductName =
                            item.Product?.Name
                            ?? string.Empty,

                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.Subtotal
                    })
                .ToList()
        };
    }
}