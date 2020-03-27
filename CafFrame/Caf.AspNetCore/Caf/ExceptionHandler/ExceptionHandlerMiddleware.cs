using AspectCore.DynamicProxy;
using Caf.Core;
using Caf.Core.DataModel.Http;
using Caf.Core.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Caf.AspNetCore.ExceptionHandler
{
    public class ExceptionHandlerMiddleware : IMiddleware, ITransient
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                try
                {
                    if(ex is AspectInvocationException)
                    {
                        ex = ex.InnerException;
                    }                  
                    _logger.LogError(ex, ex.Message);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new ExceptionResponse {IsSuccess=false,Message = ex.Message, ExceptionDetail = GetExceptionDetail(ex)},options:new JsonSerializerOptions() { PropertyNamingPolicy= JsonNamingPolicy.CamelCase }));
                }
                catch
                {
                    _logger.LogWarning("异常捕获失败");
                }
            }
        }

        private string GetExceptionDetail(Exception ex)
        {
            StringBuilder builder = new StringBuilder();
            AddExceptionToDetails(ex, builder);
            return builder.ToString();

        }
        private void AddExceptionToDetails(Exception ex, StringBuilder detailBuilder)
        {
            detailBuilder.AppendLine(ex.GetType().Name + ": " + ex.Message);

            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                detailBuilder.AppendLine("track " + ex.StackTrace);
            }
            if (ex.InnerException != null)
            {
                AddExceptionToDetails(ex.InnerException, detailBuilder);
            }
        }
    }
}
