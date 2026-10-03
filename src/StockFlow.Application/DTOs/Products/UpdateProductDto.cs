namespace StockFlow.Application.DTOs.Products;

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int MinimumStock { get; set; }

    public int CategoryId { get; set; }

    public bool IsActive { get; set; }
}