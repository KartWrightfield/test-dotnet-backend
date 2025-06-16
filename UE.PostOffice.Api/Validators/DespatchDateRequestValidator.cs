using System;
using System.Linq;
using FluentValidation;
using UE.PostOffice.Api.Models;

namespace UE.PostOffice.Api.Validators;

/// <summary>
/// Validates the despatch date request to ensure all constraints are met.
/// </summary>
/// <remarks>
/// The validation checks include:
/// - Ensuring the product IDs are not empty, contain only positive numbers greater than zero, and do not include duplicates.
/// - Ensuring the order date is provided and is not a future date.
/// </remarks>
public class DespatchDateRequestValidator : AbstractValidator<DespatchDateRequest>
{
    public DespatchDateRequestValidator()
    {
        RuleFor(x => x.ProductIds)
            .NotEmpty()
            .Must(ids => ids.All(id => id > 0))
            .WithMessage("All product IDs must be positive numbers greater than 0")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Product IDs must not contain duplicates");
        
        RuleFor(x => x.OrderDate)
            .NotEmpty()
            .Must(date => date <= DateTime.UtcNow)
            .WithMessage("Order date cannot be in the future");
    }
}