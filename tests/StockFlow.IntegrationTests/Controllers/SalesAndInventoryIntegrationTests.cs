using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using StockFlow.IntegrationTests.Infrastructure;

namespace StockFlow.IntegrationTests.Controllers;

public class SalesAndInventoryIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SalesAndInventoryIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateSale_ShouldReturnCreated_WhenDataIsValid()
    {
        var client = _factory.CreateClient();

        var token = await LoginAsync(
            client,
            "admin@stockflow.com",
            "Admin123*");

        SetBearerToken(client, token);

        var categoryId = await GetCategoryIdAsync(client);

        var productId = await CreateProductAsync(
            client,
            categoryId,
            stock: 10);

        var customerId = await CreateCustomerAsync(client);

        var request = new
        {
            customerId,
            items = new[]
            {
                new
                {
                    productId,
                    quantity = 2
                }
            }
        };

        var response = await client.PostAsJsonAsync(
            "/api/sales",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var content =
            await response.Content.ReadAsStringAsync();

        using var json =
            JsonDocument.Parse(content);

        var root = json.RootElement;

        Assert.True(
            root.GetProperty("id").GetInt32() > 0);

        Assert.Equal(
            customerId,
            root.GetProperty("customerId").GetInt32());

        Assert.Equal(
            300000m,
            root.GetProperty("total").GetDecimal());

        var items =
            root.GetProperty("items");

        Assert.Equal(
            1,
            items.GetArrayLength());

        var item =
            items[0];

        Assert.Equal(
            productId,
            item.GetProperty("productId").GetInt32());

        Assert.Equal(
            2,
            item.GetProperty("quantity").GetInt32());

        Assert.Equal(
            150000m,
            item.GetProperty("unitPrice").GetDecimal());

        Assert.Equal(
            300000m,
            item.GetProperty("subtotal").GetDecimal());

        var productResponse = await client.GetAsync(
            $"/api/products/{productId}");

        Assert.Equal(
            HttpStatusCode.OK,
            productResponse.StatusCode);

        var productContent =
            await productResponse.Content.ReadAsStringAsync();

        using var productJson =
            JsonDocument.Parse(productContent);

        Assert.Equal(
            8,
            productJson.RootElement
                .GetProperty("stock")
                .GetInt32());
    }

    [Fact]
    public async Task CreateInventoryEntry_ShouldIncreaseStock_WhenAdminIsAuthenticated()
    {
        var client = _factory.CreateClient();

        var token = await LoginAsync(
            client,
            "admin@stockflow.com",
            "Admin123*");

        SetBearerToken(client, token);

        var categoryId = await GetCategoryIdAsync(client);

        var productId = await CreateProductAsync(
            client,
            categoryId,
            stock: 5);

        var request = new
        {
            productId,
            quantity = 10,
            reference = "Integration Test Restock"
        };

        var response = await client.PostAsJsonAsync(
            "/api/inventory/entry",
            request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var content =
            await response.Content.ReadAsStringAsync();

        using var json =
            JsonDocument.Parse(content);

        var root = json.RootElement;

        Assert.True(
            root.GetProperty("id").GetInt32() > 0);

        Assert.Equal(
            productId,
            root.GetProperty("productId").GetInt32());

        Assert.Equal(
            "Entry",
            root.GetProperty("type").GetString());

        Assert.Equal(
            10,
            root.GetProperty("quantity").GetInt32());

        Assert.Equal(
            5,
            root.GetProperty("previousStock").GetInt32());

        Assert.Equal(
            15,
            root.GetProperty("newStock").GetInt32());

        Assert.Equal(
            "Integration Test Restock",
            root.GetProperty("reference").GetString());

        var productResponse = await client.GetAsync(
            $"/api/products/{productId}");

        Assert.Equal(
            HttpStatusCode.OK,
            productResponse.StatusCode);

        var productContent =
            await productResponse.Content.ReadAsStringAsync();

        using var productJson =
            JsonDocument.Parse(productContent);

        Assert.Equal(
            15,
            productJson.RootElement
                .GetProperty("stock")
                .GetInt32());
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

    private static void SetBearerToken(
        HttpClient client,
        string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);
    }

    private static async Task<int> GetCategoryIdAsync(
        HttpClient client)
    {
        var response =
            await client.GetAsync("/api/categories");

        response.EnsureSuccessStatusCode();

        var content =
            await response.Content.ReadAsStringAsync();

        using var json =
            JsonDocument.Parse(content);

        var categories =
            json.RootElement;

        if (categories.GetArrayLength() == 0)
        {
            throw new InvalidOperationException(
                "No categories were found.");
        }

        return categories[0]
            .GetProperty("id")
            .GetInt32();
    }

    private static async Task<int> CreateProductAsync(
        HttpClient client,
        int categoryId,
        int stock)
    {
        var sku =
            $"TEST-{Guid.NewGuid():N}";

        var request = new
        {
            name = "Integration Test Product",
            sku,
            description = "Product created by integration test",
            price = 150000m,
            stock,
            minimumStock = 2,
            categoryId
        };

        var response = await client.PostAsJsonAsync(
            "/api/products",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var content =
            await response.Content.ReadAsStringAsync();

        using var json =
            JsonDocument.Parse(content);

        return json.RootElement
            .GetProperty("id")
            .GetInt32();
    }

    private static async Task<int> CreateCustomerAsync(
        HttpClient client)
    {
        var document =
            $"DOC-{Guid.NewGuid():N}";

        var request = new
        {
            name = "Integration Test Customer",
            document,
            email = "integration@test.com",
            phone = "3001234567"
        };

        var response = await client.PostAsJsonAsync(
            "/api/customers",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var content =
            await response.Content.ReadAsStringAsync();

        using var json =
            JsonDocument.Parse(content);

        return json.RootElement
            .GetProperty("id")
            .GetInt32();
    }
}