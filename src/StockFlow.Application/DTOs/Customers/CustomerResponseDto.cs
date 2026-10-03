namespace StockFlow.Application.DTOs.Customers;

public class CustomerResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Document { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }
}