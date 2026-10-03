namespace StockFlow.Application.DTOs.Sales;

public class CreateSaleDto
{
    public int CustomerId { get; set; }

    public List<CreateSaleItemDto> Items { get; set; } = new();
}