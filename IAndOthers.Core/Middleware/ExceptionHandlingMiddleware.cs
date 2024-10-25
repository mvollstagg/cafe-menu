using IAndOthers.Core.Data.Enumeration;
using IAndOthers.Core.Data.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace IAndOthers.Core.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred.");

            context.Response.ContentType = "application/json";
            IOResult<string> ioResult;

            switch (exception)
            {
                case ArgumentException argEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ioResult = new IOResult<string>(IOResultStatusEnum.Error, "Invalid argument provided.");

                    var argumentError = ioResult.Meta.Messages
                        .FirstOrDefault(kvp => kvp.Key == "ArgumentException");

                    if (argumentError.Equals(default(KeyValuePair<string, List<string>>)))
                    {
                        ioResult.Meta.Messages.Add(
                            new KeyValuePair<string, List<string>>("ArgumentException", new List<string> { argEx.Message })
                        );
                    }
                    else
                    {
                        argumentError.Value.Add(argEx.Message);
                    }
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    ioResult = new IOResult<string>(IOResultStatusEnum.Error, "Unauthorized access.");
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    ioResult = new IOResult<string>(IOResultStatusEnum.Error, "An unexpected error occurred.");

                    var exceptionError = ioResult.Meta.Messages
                        .FirstOrDefault(kvp => kvp.Key == "Exception");

                    if (exceptionError.Equals(default(KeyValuePair<string, List<string>>)))
                    {
                        ioResult.Meta.Messages.Add(
                            new KeyValuePair<string, List<string>>("Exception", new List<string> { exception.Message })
                        );
                    }
                    else
                    {
                        exceptionError.Value.Add(exception.Message);
                    }
                    break;
            }

            var result = JsonConvert.SerializeObject(ioResult);
            await context.Response.WriteAsync(result);
        }
    }
}
