using Caf.AppSetting.DbContextService;
using Caf.AppSetting.ServiceCollectionExtention;
using Caf.Cache;
using Caf.Core.AppSetting;
using Caf.Core.DependencyInjection;
using Caf.Core.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;

namespace Caf.AppSetting
{
    [UsingModule(typeof(CafCacheModule))]
    public class CafAppSettingModule : CafModule
    {
        public override void ConfigureServices(CafConfigurationContext context)
        {
            JudgeConfigureConn(context);
            context.Services.AddSingleton(typeof(AppSettingsConfigure));
            context.Services.AddSingleton<IAppSettingsService, AppSettingsService>();

            context.Services.Configure<AccountOption>(context.Configuration.GetSection(
                                        nameof(AccountOption)));
        }
        public override void OnApplicationInitialization(CafApplicationContext context)
        {
            context.ServiceProvider.GetService<AppSettingsConfigure>()?.Init();
            //ui
            var app = context.ServiceProvider.GetRequiredService<ObjectWrapper<IApplicationBuilder>>().Value;
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.ToLower().StartsWith("/appsettings"))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
                else
                {
                    await next();
                }

            });

            var manifestEmbeddedProvider =
                new ManifestEmbeddedFileProvider(typeof(CafAppSettingModule).Assembly, "UI");
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = manifestEmbeddedProvider
            });
        }

        private void JudgeConfigureConn(CafConfigurationContext context) 
        {
            //notice:使用方需要指定配置AppSettingsConnection
            //string conn = context.Configuration.GetSection("ConnectionStrings")["AppSettingsConnection"];

            var conn = context.Services.BuildServiceProvider().GetService<IOptions<ConnectionStrings>>().Value.AppSettingsConnection;
            if (string.IsNullOrWhiteSpace(conn))
            {
                conn = context.Configuration.GetSection("ConnectionStrings")["AppSettingsConnection"];
            }
            try
            {
                context.Services.AddDbContext<CafAppsettingDbContext>(options => options.UseSqlServer(conn));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
