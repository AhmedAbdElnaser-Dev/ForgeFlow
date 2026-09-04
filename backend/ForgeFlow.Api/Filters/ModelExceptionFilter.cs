using ForgeFlow.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ForgeFlow.Api.Filters;

// Maps model/folder failures to status codes once, so controllers stay free of try/catch.
public class ModelExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            BucketNotActiveException notActive => new ConflictObjectResult(new ProblemDetails
            {
                Title = "Folder is not active",
                Detail = notActive.Message,
                Status = StatusCodes.Status409Conflict,
            }),
            ModelNotFoundException => new NotFoundResult(),
            _ => null!,
        };

        if (context.Result is not null)
        {
            context.ExceptionHandled = true;
        }
    }
}
