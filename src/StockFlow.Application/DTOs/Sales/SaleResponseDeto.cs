namespace StockFlow.Application.DTOs.Sales;

public class SaleResponseDto
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<SaleItemResponseDto> Items { get; set; } = new();
}