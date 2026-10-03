using Moq;
using StockFlow.Application.DTOs.Sales;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Application.Services;
using StockFlow.Domain.Entities;

namespace StockFlow.UnitTests.Services;

public class SaleServiceTests
{
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly SaleService _saleService;

    public SaleServiceTests()
    {
        _saleRepositoryMock = new Mock<ISaleRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _saleService = new SaleService(
            _saleRepositoryMock.Object,
            _productRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _userRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateSale_WhenDataIsValid()
    {
        var customer = new Customer
        {
            Id = 1,
            Name = "Juan Perez",
            Document = "123456"
        };

        var user = new User
        {
            Id = 1,
            Name = "StockFlow Admin",
            Email = "admin@stockflow.com"
        };

        var product = new Product
        {
            Id = 1,
            Name = "Mechanical Pro Max",
            Sku = "KEY-001",
            Price = 275000,
            Stock = 10,
            IsActive = true
        };

        var dto = new CreateSaleDto
        {
            CustomerId = 1,
            Items = new List<CreateSaleItemDto>
            {
                new()
                {
                    ProductId = 1,
                    Quantity = 2
                }
            }
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(customer);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        _saleRepositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<Sale>(),
                It.IsAny<IEnumerable<InventoryMovement>>()))
            .Callback<Sale, IEnumerable<InventoryMovement>>(
                (sale, movements) =>
                {
                    sale.Id = 1;
                })
            .Returns(Task.CompletedTask);

        _saleRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(() =>
            {
                return new Sale
                {
                    Id = 1,
                    CustomerId = customer.Id,
                    Customer = customer,
                    UserId = user.Id,
                    User = user,
                    Total = 550000,
                    Items = new List<SaleItem>
                    {
                        new()
                        {
                            ProductId = product.Id,
                            Product = product,
                            Quantity = 2,
                            UnitPrice = product.Price,
                            Subtotal = 550000
                        }
                    }
                };
            });

        var result = await _saleService.CreateAsync(dto, 1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(550000, result.Total);
        Assert.Single(result.Items);
        Assert.Equal(8, product.Stock);

        _saleRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Sale>(),
                It.IsAny<IEnumerable<InventoryMovement>>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenCustomerDoesNotExist()
    {
        var dto = new CreateSaleDto
        {
            CustomerId = 999,
            Items = new List<CreateSaleItemDto>
            {
                new()
                {
                    ProductId = 1,
                    Quantity = 1
                }
            }
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Customer?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _saleService.CreateAsync(dto, 1));

        Assert.Equal(
            "Customer not found.",
            exception.Message);

        _saleRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Sale>(),
                It.IsAny<IEnumerable<InventoryMovement>>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenProductDoesNotExist()
    {
        var customer = new Customer
        {
            Id = 1,
            Name = "Juan Perez"
        };

        var user = new User
        {
            Id = 1,
            Name = "StockFlow Admin"
        };

        var dto = new CreateSaleDto
        {
            CustomerId = 1,
            Items = new List<CreateSaleItemDto>
            {
                new()
                {
                    ProductId = 999,
                    Quantity = 1
                }
            }
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(customer);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _saleService.CreateAsync(dto, 1));

        Assert.Contains(
            "Product with id 999 was not found.",
            exception.Message);

        _saleRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Sale>(),
                It.IsAny<IEnumerable<InventoryMovement>>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenStockIsInsufficient()
    {
        var customer = new Customer
        {
            Id = 1,
            Name = "Juan Perez"
        };

        var user = new User
        {
            Id = 1,
            Name = "StockFlow Admin"
        };

        var product = new Product
        {
            Id = 1,
            Name = "Mechanical Pro Max",
            Price = 275000,
            Stock = 2,
            IsActive = true
        };

        var dto = new CreateSaleDto
        {
            CustomerId = 1,
            Items = new List<CreateSaleItemDto>
            {
                new()
                {
                    ProductId = 1,
                    Quantity = 5
                }
            }
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(customer);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _saleService.CreateAsync(dto, 1));

        Assert.Contains(
            "Insufficient stock",
            exception.Message);

        Assert.Equal(2, product.Stock);

        _saleRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Sale>(),
                It.IsAny<IEnumerable<InventoryMovement>>()),
            Times.Never);
    }
}