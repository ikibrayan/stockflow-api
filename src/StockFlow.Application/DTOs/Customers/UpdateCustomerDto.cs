namespace StockFlow.Application.DTOs.Customers;

public class UpdateCustomerDto
{
    public string Name { get; set; } = string.Empty;

    public string Document { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }
}