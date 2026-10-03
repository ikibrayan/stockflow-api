using Moq;
using StockFlow.Application.DTOs.Inventory;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Application.Services;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;

namespace StockFlow.UnitTests.Services;

public class InventoryServiceTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly InventoryService _inventoryService;

    public InventoryServiceTests()
    {
        _inventoryRepositoryMock = new Mock<IInventoryRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _inventoryService = new InventoryService(
            _inventoryRepositoryMock.Object,
            _productRepositoryMock.Object,
            _userRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateEntryAsync_ShouldIncreaseStock_WhenDataIsValid()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Mechanical Pro Max",
            Stock = 10,
            IsActive = true
        };

        var user = new User
        {
            Id = 1,
            Name = "StockFlow Admin"
        };

        var dto = new CreateInventoryEntryDto
        {
            ProductId = 1,
            Quantity = 5,
            Reference = "Restock Test"
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        _inventoryRepositoryMock
            .Setup(x => x.SaveMovementAsync(
                It.IsAny<Product>(),
                It.IsAny<InventoryMovement>()))
            .Callback<Product, InventoryMovement>(
                (savedProduct, movement) =>
                {
                    movement.Id = 1;
                })
            .Returns(Task.CompletedTask);

        _inventoryRepositoryMock
            .Setup(x => x.GetByProductIdAsync(1))
            .ReturnsAsync(() =>
                new List<InventoryMovement>
                {
                    new()
                    {
                        Id = 1,
                        ProductId = product.Id,
                        Product = product,
                        UserId = user.Id,
                        User = user,
                        Type = InventoryMovementType.Entry,
                        Quantity = 5,
                        PreviousStock = 10,
                        NewStock = 15,
                        Reference = "Restock Test"
                    }
                });

        var result =
            await _inventoryService.CreateEntryAsync(dto, 1);

        Assert.NotNull(result);
        Assert.Equal(15, product.Stock);
        Assert.Equal(10, result.PreviousStock);
        Assert.Equal(15, result.NewStock);
        Assert.Equal(5, result.Quantity);
        Assert.Equal("Entry", result.Type);

        _inventoryRepositoryMock.Verify(
            x => x.SaveMovementAsync(
                It.Is<Product>(p =>
                    p.Id == 1 &&
                    p.Stock == 15),
                It.Is<InventoryMovement>(m =>
                    m.Type == InventoryMovementType.Entry &&
                    m.Quantity == 5 &&
                    m.PreviousStock == 10 &&
                    m.NewStock == 15 &&
                    m.Reference == "Restock Test")),
            Times.Once);
    }

    [Fact]
    public async Task CreateAdjustmentAsync_ShouldAdjustStock_WhenDataIsValid()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Mechanical Pro Max",
            Stock = 20,
            IsActive = true
        };

        var user = new User
        {
            Id = 1,
            Name = "StockFlow Admin"
        };

        var dto = new CreateInventoryAdjustmentDto
        {
            ProductId = 1,
            NewStock = 15,
            Reference = "Physical Count"
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        _inventoryRepositoryMock
            .Setup(x => x.SaveMovementAsync(
                It.IsAny<Product>(),
                It.IsAny<InventoryMovement>()))
            .Callback<Product, InventoryMovement>(
                (savedProduct, movement) =>
                {
                    movement.Id = 2;
                })
            .Returns(Task.CompletedTask);

        _inventoryRepositoryMock
            .Setup(x => x.GetByProductIdAsync(1))
            .ReturnsAsync(() =>
                new List<InventoryMovement>
                {
                    new()
                    {
                        Id = 2,
                        ProductId = product.Id,
                        Product = product,
                        UserId = user.Id,
                        User = user,
                        Type = InventoryMovementType.Adjustment,
                        Quantity = 5,
                        PreviousStock = 20,
                        NewStock = 15,
                        Reference = "Physical Count"
                    }
                });

        var result =
            await _inventoryService.CreateAdjustmentAsync(dto, 1);

        Assert.NotNull(result);
        Assert.Equal(15, product.Stock);
        Assert.Equal(20, result.PreviousStock);
        Assert.Equal(15, result.NewStock);
        Assert.Equal(5, result.Quantity);
        Assert.Equal("Adjustment", result.Type);

        _inventoryRepositoryMock.Verify(
            x => x.SaveMovementAsync(
                It.Is<Product>(p =>
                    p.Id == 1 &&
                    p.Stock == 15),
                It.Is<InventoryMovement>(m =>
                    m.Type == InventoryMovementType.Adjustment &&
                    m.Quantity == 5 &&
                    m.PreviousStock == 20 &&
                    m.NewStock == 15 &&
                    m.Reference == "Physical Count")),
            Times.Once);
    }

    [Fact]
    public async Task CreateEntryAsync_ShouldThrow_WhenProductDoesNotExist()
    {
        var dto = new CreateInventoryEntryDto
        {
            ProductId = 999,
            Quantity = 5,
            Reference = "Restock Test"
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _inventoryService.CreateEntryAsync(dto, 1));

        Assert.Equal(
            "Product not found.",
            exception.Message);

        _inventoryRepositoryMock.Verify(
            x => x.SaveMovementAsync(
                It.IsAny<Product>(),
                It.IsAny<InventoryMovement>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAdjustmentAsync_ShouldThrow_WhenNewStockEqualsCurrentStock()
    {
        var product = new Product
        {
            Id = 1,
            Name = "Mechanical Pro Max",
            Stock = 15,
            IsActive = true
        };

        var user = new User
        {
            Id = 1,
            Name = "StockFlow Admin"
        };

        var dto = new CreateInventoryAdjustmentDto
        {
            ProductId = 1,
            NewStock = 15,
            Reference = "Physical Count"
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _inventoryService.CreateAdjustmentAsync(
                    dto,
                    1));

        Assert.Equal(
            "The new stock is equal to the current stock.",
            exception.Message);

        Assert.Equal(15, product.Stock);

        _inventoryRepositoryMock.Verify(
            x => x.SaveMovementAsync(
                It.IsAny<Product>(),
                It.IsAny<InventoryMovement>()),
            Times.Never);
    }
}