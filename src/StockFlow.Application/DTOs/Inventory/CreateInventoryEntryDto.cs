namespace StockFlow.Application.DTOs.Inventory;

public class CreateInventoryEntryDto
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string? Reference { get; set; }
}