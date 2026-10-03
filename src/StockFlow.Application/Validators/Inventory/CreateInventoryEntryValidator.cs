using FluentValidation;
using StockFlow.Application.DTOs.Inventory;

namespace StockFlow.Application.Validators.Inventory;

public class CreateInventoryEntryValidator
    : AbstractValidator<CreateInventoryEntryDto>
{
    public CreateInventoryEntryValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.Reference)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Reference));
    }
}