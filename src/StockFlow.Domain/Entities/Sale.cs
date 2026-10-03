using StockFlow.Domain.Common;

namespace StockFlow.Domain.Entities;

public class Sale : BaseEntity
{
    public int CustomerId { get; set; }

    public int UserId { get; set; }

    public decimal Total { get; set; }

    public Customer Customer { get; set; } = null!;

    public User User { get; set; } = null!;

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}