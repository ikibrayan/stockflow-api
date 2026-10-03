using StockFlow.Application.DTOs.Inventory;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Application.Interfaces.Services;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;

    public InventoryService(
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        IUserRepository userRepository)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
    }

    public async Task<List<InventoryMovementResponseDto>>
        GetAllAsync()
    {
        var movements =
            await _inventoryRepository.GetAllAsync();

        return movements
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<InventoryMovementResponseDto>>
        GetByProductIdAsync(int productId)
    {
        var product =
            await _productRepository.GetByIdAsync(productId);

        if (product is null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        var movements =
            await _inventoryRepository
                .GetByProductIdAsync(productId);

        return movements
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<InventoryMovementResponseDto>
        CreateEntryAsync(
            CreateInventoryEntryDto dto,
            int userId)
    {
        var product =
            await _productRepository.GetByIdAsync(
                dto.ProductId);

        if (product is null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        var user =
            await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        var previousStock = product.Stock;
        var newStock = previousStock + dto.Quantity;

        product.Stock = newStock;

        var movement = new InventoryMovement
        {
            ProductId = product.Id,
            UserId = userId,
            Type = InventoryMovementType.Entry,
            Quantity = dto.Quantity,
            PreviousStock = previousStock,
            NewStock = newStock,
            Reference = dto.Reference
        };

        await _inventoryRepository.SaveMovementAsync(
            product,
            movement);

        var createdMovements =
            await _inventoryRepository
                .GetByProductIdAsync(product.Id);

        var createdMovement =
            createdMovements.First(
                x => x.Id == movement.Id);

        return MapToResponse(createdMovement);
    }

    public async Task<InventoryMovementResponseDto>
        CreateAdjustmentAsync(
            CreateInventoryAdjustmentDto dto,
            int userId)
    {
        var product =
            await _productRepository.GetByIdAsync(
                dto.ProductId);

        if (product is null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        var user =
            await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User not found.");
        }

        var previousStock = product.Stock;

        if (previousStock == dto.NewStock)
        {
            throw new InvalidOperationException(
                "The new stock is equal to the current stock.");
        }

        product.Stock = dto.NewStock;

        var quantity =
            Math.Abs(dto.NewStock - previousStock);

        var movement = new InventoryMovement
        {
            ProductId = product.Id,
            UserId = userId,
            Type = InventoryMovementType.Adjustment,
            Quantity = quantity,
            PreviousStock = previousStock,
            NewStock = dto.NewStock,
            Reference = dto.Reference
        };

        await _inventoryRepository.SaveMovementAsync(
            product,
            movement);

        var createdMovements =
            await _inventoryRepository
                .GetByProductIdAsync(product.Id);

        var createdMovement =
            createdMovements.First(
                x => x.Id == movement.Id);

        return MapToResponse(createdMovement);
    }

    private static InventoryMovementResponseDto MapToResponse(
        InventoryMovement movement)
    {
        return new InventoryMovementResponseDto
        {
            Id = movement.Id,
            ProductId = movement.ProductId,

            ProductName =
                movement.Product?.Name ?? string.Empty,

            UserId = movement.UserId,

            UserName =
                movement.User?.Name ?? string.Empty,

            Type = movement.Type.ToString(),

            Quantity = movement.Quantity,

            PreviousStock = movement.PreviousStock,

            NewStock = movement.NewStock,

            Reference = movement.Reference,

            CreatedAt = movement.CreatedAt
        };
    }
}