using StockFlow.Application.DTOs.Products;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Application.Interfaces.Services;
using StockFlow.Domain.Entities;

namespace StockFlow.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(MapToResponse);
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            return null;

        return MapToResponse(product);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        var skuExists = await _productRepository.ExistsBySkuAsync(dto.Sku);

        if (skuExists)
            throw new InvalidOperationException(
                "A product with this SKU already exists.");

        var product = new Product
        {
            Name = dto.Name,
            Sku = dto.Sku,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            MinimumStock = dto.MinimumStock,
            CategoryId = dto.CategoryId,
            IsActive = true
        };

        await _productRepository.AddAsync(product);

        var createdProduct =
            await _productRepository.GetByIdAsync(product.Id);

        if (createdProduct is null)
            throw new InvalidOperationException(
                "The product could not be retrieved after creation.");

        return MapToResponse(createdProduct);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            return false;

        if (!string.Equals(product.Sku, dto.Sku, StringComparison.OrdinalIgnoreCase))
        {
            var skuExists = await _productRepository.ExistsBySkuAsync(dto.Sku);

            if (skuExists)
                throw new InvalidOperationException("A product with this SKU already exists.");
        }

        product.Name = dto.Name;
        product.Sku = dto.Sku;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.MinimumStock = dto.MinimumStock;
        product.CategoryId = dto.CategoryId;
        product.IsActive = dto.IsActive;

        await _productRepository.UpdateAsync(product);

        return true;
    }

    private static ProductResponseDto MapToResponse(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            MinimumStock = product.MinimumStock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }
}