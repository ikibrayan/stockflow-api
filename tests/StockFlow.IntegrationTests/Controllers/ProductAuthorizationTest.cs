using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using StockFlow.IntegrationTests.Infrastructure;

namespace StockFlow.IntegrationTests.Controllers;

public class ProductsAuthorizationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ProductsAuthorizationTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProducts_ShouldReturnUnauthorized_WhenTokenIsMissing()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(
            "/api/products");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk_WhenAdminIsAuthenticated()
    {
        var client = _factory.CreateClient();

        var token = await LoginAsync(
            client,
            "admin@stockflow.com",
            "Admin123*");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var response = await client.GetAsync(
            "/api/products");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnForbidden_WhenSellerIsAuthenticated()
    {
        var client = _factory.CreateClient();

        var token = await LoginAsync(
            client,
            "seller@stockflow.com",
            "Seller123*");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var request = new
        {
            name = "Seller Product Test",
            sku = "SELLER-TEST-001",
            description = "Integration test",
            price = 100000,
            stock = 10,
            minimumStock = 2,
            categoryId = 1
        };

        var response = await client.PostAsJsonAsync(
            "/api/products",
            request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated_WhenAdminIsAuthenticated()
    {
        var client = _factory.CreateClient();

        var token = await LoginAsync(
            client,
            "admin@stockflow.com",
            "Admin123*");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var request = new
        {
            name = "Admin Product Test",
            sku = $"ADMIN-TEST-{Guid.NewGuid():N}",
            description = "Integration test",
            price = 150000,
            stock = 10,
            minimumStock = 2,
            categoryId = 1
        };

        var response = await client.PostAsJsonAsync(
            "/api/products",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    private static async Task<string> LoginAsync(
        HttpClient client,
        string email,
        string password)
    {
        var request = new
        {
            email,
            password
        };

        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            request);

        response.EnsureSuccessStatusCode();

        var content =
            await response.Content.ReadAsStringAsync();

        using var json =
            JsonDocument.Parse(content);

        return json.RootElement
            .GetProperty("token")
            .GetString()
            ?? throw new InvalidOperationException(
                "Token was not returned.");
    }
}