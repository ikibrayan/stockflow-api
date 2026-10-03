namespace StockFlow.Application.DTOs.Inventory;

public class CreateInventoryAdjustmentDto
{
    public int ProductId { get; set; }

    public int NewStock { get; set; }

    public string? Reference { get; set; }
}