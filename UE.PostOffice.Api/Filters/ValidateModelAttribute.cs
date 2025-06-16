using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UE.PostOffice.Api.Filters;

/// <summary>
/// Represents an action filter attribute that validates the model arguments of an action method.
/// </summary>
/// <remarks>
/// This attribute is primarily used to ensure that incoming data for a controller action is validated
/// when a corresponding validator for the data type is provided. It integrates with the FluentValidation
/// library to perform the validation process.
/// </remarks>
/// <example>
/// If the validation fails, the filter sets the response result to a <see cref="BadRequestObjectResult"/>
/// containing the validation errors.
/// </example>
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var serviceProvider = context.HttpContext.RequestServices;

        foreach (var argument in context.ActionArguments.Values)
        {
            var argumentType = argument?.GetType();
            if (argumentType == null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            if (serviceProvider.GetService(validatorType) is IValidator validator)
            {
                var validationContext = new ValidationContext<object>(argument);
                var validationResult = validator.Validate(validationContext);

                if (!validationResult.IsValid)
                {
                    context.Result = new BadRequestObjectResult(validationResult.Errors);
                    return;
                }
            }
        }

    }
}