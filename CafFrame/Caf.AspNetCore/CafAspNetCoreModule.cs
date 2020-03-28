using Caf.Core.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caf.AspNetCore.Caf.Cors;
using Microsoft.Extensions.Options;
using Capgemini.Caf;
using Caf.Core.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Caf.Core;
using Caf.Core.Caf.File;

namespace Capgemini.Frame.AspNetCore
{
    
    public class CafAspNetCoreModule : CafModule
    {
        private const string _defaultCorsPolicyName = "_defaultCorsPolicyName";
        public override void BeforeConfigureServices(CafConfigurationContext context)
        {
            //context.Services.AddObjectWrapper(new CafCorsOption());
        }
        public override void ConfigureServices(CafConfigurationContext context)
        {
            AddAspNetServices(context.Services);
            context.ConfigFileOptions();
            AddAspNetCors(context);
            context.Services.AddObjectWrapper<IApplicationBuilder>();
        }
        public override void OnApplicationInitialization(CafApplicationContext context)
        {
            var app = context.GetApplicationBuilder();
            app.Use(async (context, next) => {
                context.Request.EnableBuffering();
                await next();
            });
            app.UseCors(_defaultCorsPolicyName);
            app.UseStaticHttpContext();
        }
        private static void AddAspNetServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
        }
        /// <summary>
        /// 添加跨域设置
        /// </summary>
        /// <param name="context"></param>
        private static void AddAspNetCors(CafConfigurationContext context)
        {
            var corsOption = context.Services.GetSingletonInstance<IObjectWrapper<CafCorsOption>>().Value;
            if (corsOption.Enable)
            {
                if (!string.IsNullOrEmpty(corsOption.ConfigurationSection))
                {
                    var corsFromAppsettings = context.Configuration["App:CorsOrigins"]
                                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                        .Select(o => o.RemovePostFix("/"))
                                        .ToArray();
                    corsOption.Origins.AddRange(corsFromAppsettings);
                }
                context.Services.AddCors(
                    options => options.AddPolicy(
                        _defaultCorsPolicyName,
                        builder => builder
                            .WithOrigins(
                            corsOption.Origins.Distinct().ToArray()
                                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()));
            }
        }
    }
}
