using System;
using System.Net;
using Marketplace.Helpers.Exceptions.Models;
using Marketplace.Helpers.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Marketplace.Helpers.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
                context.Result = new ValidationFailedResult(context.ModelState);
            base.OnActionExecuting(context);
        }
    }

    public class ValidationFailedResult : ObjectResult
    {
        public ValidationFailedResult(ModelStateDictionary state) : base(new ApiValidationError(state))
        {
            StatusCode = HttpStatusCode.BadRequest.To<int>();
        }
    }
}
