using System;
using System.Linq;
using FluentValidation;
using UE.PostOffice.Api.Model;

namespace UE.PostOffice.Api.Validators;

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
            .Must(date => date.Date <= DateTime.UtcNow.Date)
            .WithMessage("Order date cannot be in the future");
    }
}