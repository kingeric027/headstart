using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Orchestration.Common.Exceptions;
using Orchestration.Common.Extensions;

namespace Orchestration.Common.Helpers
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

    [AttributeUsage(AttributeTargets.Property)]
    public class SortableAttribute : Attribute
    {
        public int? Priority { get; set; }
        public bool Descending { get; set; }

        public SortableAttribute() { }

        public SortableAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
