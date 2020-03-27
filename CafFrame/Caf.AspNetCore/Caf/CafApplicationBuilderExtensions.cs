using Caf.Core.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Caf.Core;
using Caf.AspNetCore.ExceptionHandler;
using Microsoft.AspNetCore.Http;

namespace Capgemini.Caf
{
    public static class CafApplicationBuilderExtensions
    {
        public static void ApplicationRun(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetRequiredService<ObjectWrapper<IApplicationBuilder>>().Value = app;
            app.ApplicationServices.GetRequiredService<ICafApplicationServiceProvider>().Run(app.ApplicationServices);
        }
        //public static IApplicationBuilder UseAuditing(this IApplicationBuilder app)
        //{
        //    return app
        //        .UseMiddleware<AuditingMiddleware>();
        //}
        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            System.Web.HttpContext.Configure(httpContextAccessor);
            return app;
        }
        public static IApplicationBuilder UseCafExceptionHandler(this IApplicationBuilder app)
        {
            return app
                .UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
