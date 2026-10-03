using StockFlow.Domain.Common;

namespace StockFlow.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Document { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}