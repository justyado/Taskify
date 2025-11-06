using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Taskify.Application.Common;

namespace Taskify.Api.Extensions;

public class ResultActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        if (executedContext.Result is ObjectResult objectResult)
        {
            if (objectResult.Value is Result result)
            {
                object? data = null;

                var type = result.GetType();
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    data = type.GetProperty("Value")?.GetValue(result);
                }

                executedContext.Result = result.IsFailed
                    ? new BadRequestObjectResult(new { isSuccess = false, isFailed = true, errors = result.Errors })
                    : new OkObjectResult(new { isSuccess = true, isFailed = false, value = data });
            }
        }
    }
}
