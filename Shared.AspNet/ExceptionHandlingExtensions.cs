using Microsoft.AspNetCore.Builder;

namespace Shared.AspNet
{
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseSharedExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
