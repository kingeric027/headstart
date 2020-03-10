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

            if (ex is ApiErrorException apiErrorException)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = apiErrorException.ApiError.StatusCode.To<int>();
                return context.Response.WriteAsync(JsonConvert.SerializeObject(apiErrorException.ApiError));
            }
            if (ex is OrderCloudException ocException)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ocException.HttpStatus.To<int>();
                return context.Response.WriteAsync(JsonConvert.SerializeObject(ocException.Errors));
            }

            var result = JsonConvert.SerializeObject(ex);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
