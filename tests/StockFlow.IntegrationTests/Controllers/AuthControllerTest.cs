using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using StockFlow.IntegrationTests.Infrastructure;

namespace StockFlow.IntegrationTests.Controllers;

public class AuthControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var request = new
        {
            email = "admin@stockflow.com",
            password = "Admin123*"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
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
            root.TryGetProperty(
                "token",
                out var token));

        Assert.False(
            string.IsNullOrWhiteSpace(
                token.GetString()));

        Assert.Equal(
            "admin@stockflow.com",
            root.GetProperty("email").GetString());

        Assert.Equal(
            "Admin",
            root.GetProperty("role").GetString());
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        var request = new
        {
            email = "admin@stockflow.com",
            password = "WrongPassword"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            request);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}