using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Shared.Exceptions.Exceptions;
using System.Net;

namespace Shared.AspNet
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    logger.LogError(ex, "Exception after response started for {Method} {Path}",
                        context.Request.Method, context.Request.Path);
                    throw;
                }
                if (ex is UnauthorizedAccessException or AppException or ArgumentException or InvalidOperationException)
                {
                    logger.LogWarning("Business rule violation: {Message}", ex.Message);
                }
                else
                {
                    logger.LogError(ex, "An unhandled exception occurred during request {Method} {Path}",
                        context.Request.Method, context.Request.Path);
                }
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = exception switch
            {
                AppException appEx => appEx.StatusCode,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                ArgumentException or InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var statusCode = context.Response.StatusCode;

            var response = new ProblemDetails
            {
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
                Status = statusCode,
                Detail = statusCode == StatusCodes.Status500InternalServerError ? "Server side error" : exception.Message,
                Instance = context.Request.Path
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}