using System;
using System.Net;
using System.Threading.Tasks;
using Marketplace.Helpers.Exceptions;
using Marketplace.Helpers.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OrderCloud.SDK;

namespace Marketplace.Helpers
{
    public class GlobalExceptionHandler : IMiddleware
    {

        public GlobalExceptionHandler()
        {
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            const HttpStatusCode code = HttpStatusCode.InternalServerError; // 500 if unexpected
            context.Response.ContentType = "application/json";

            if (ex is ApiErrorException apiErrorException)
            {
                context.Response.StatusCode = apiErrorException.ApiError.StatusCode.To<int>();
                return context.Response.WriteAsync(JsonConvert.SerializeObject(apiErrorException.ApiError));
            }
            if (ex is OrderCloudException ocException)
            {
                context.Response.StatusCode = ocException.HttpStatus.To<int>();
                return context.Response.WriteAsync(JsonConvert.SerializeObject(ocException.Errors));
            }

            // this is only to be hit IF it's not handled properly in the stack. It's considered a bug if ever hits this. that's why it's a 500
            var result = JsonConvert.SerializeObject(new ApiError()
            {
                Data = ex.Message,
                ErrorCode = code.ToString(),
                Message = $"Unknown error has occured."
            });
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
