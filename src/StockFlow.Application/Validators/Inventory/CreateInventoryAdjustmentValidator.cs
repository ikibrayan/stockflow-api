using FluentValidation;
using StockFlow.Application.DTOs.Inventory;

namespace StockFlow.Application.Validators.Inventory;

public class CreateInventoryAdjustmentValidator
    : AbstractValidator<CreateInventoryAdjustmentDto>
{
    public CreateInventoryAdjustmentValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.NewStock)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Reference)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Reference));
    }
}