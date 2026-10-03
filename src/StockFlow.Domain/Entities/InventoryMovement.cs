using StockFlow.Domain.Common;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Entities;

public class InventoryMovement : BaseEntity
{
    public int ProductId { get; set; }

    public int UserId { get; set; }

    public InventoryMovementType Type { get; set; }

    public int Quantity { get; set; }

    public int PreviousStock { get; set; }

    public int NewStock { get; set; }

    public string? Reference { get; set; }

    public Product Product { get; set; } = null!;

    public User User { get; set; } = null!;
}