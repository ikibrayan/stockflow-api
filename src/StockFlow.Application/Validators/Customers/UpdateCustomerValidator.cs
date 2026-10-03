using FluentValidation;
using StockFlow.Application.DTOs.Customers;

namespace StockFlow.Application.Validators.Customers;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(200)
            .WithMessage("Customer name cannot exceed 200 characters.");

        RuleFor(x => x.Document)
            .NotEmpty()
            .WithMessage("Document is required.")
            .MaximumLength(50)
            .WithMessage("Document cannot exceed 50 characters.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .MaximumLength(200)
            .WithMessage("Email cannot exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(50)
            .WithMessage("Phone cannot exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}