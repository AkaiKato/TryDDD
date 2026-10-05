using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shared.AspNet
{
    public class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var argumentType = argument.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

                if (serviceProvider.GetService(validatorType) is not IValidator validator) continue;

                var validationContext = new ValidationContext<object>(argument);
                var validationResult = await validator.ValidateAsync(validationContext);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(z => z.ErrorMessage).ToArray()
                        );

                    var problemDetails = new ProblemDetails
                    {
                        Title = "Validation failed",
                        Status = 400,
                        Detail = "One or more validation errors occurred.",
                        Instance = context.HttpContext.Request.Path,
                        Extensions = { ["errors"] = errors }
                    };

                    context.Result = new BadRequestObjectResult(problemDetails);
                    return;
                }
            }

            await next();
        }
    }
}
