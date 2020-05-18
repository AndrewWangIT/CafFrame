using Caf.Core.DependencyInjection;
using Caf.Core.Module;
using Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using System;

namespace Caf.Job
{
    public class CafJobModule : CafModule
    {
        public override void ConfigureServices(CafConfigurationContext context)
        {
            context.Services.AddSingleton(GetScheduler(context));
        }
        public override void OnApplicationInitialization(CafApplicationContext context)
        {
            context.ServiceProvider.GetService<SchedulerCenter>()?.Init();//初始化Job
            var app = context.ServiceProvider.GetRequiredService<ObjectWrapper<IApplicationBuilder>>().Value;
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.ToLower().StartsWith("/jobui"))
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
                new ManifestEmbeddedFileProvider(typeof(CafJobModule).Assembly, "UI");
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = manifestEmbeddedProvider
            });
        }
        private SchedulerCenter GetScheduler(CafConfigurationContext context)
        {
            string dbProviderName = context.Configuration.GetSection("Quartz")["dbProviderName"];
            string connectionString = context.Configuration.GetSection("Quartz")["connectionString"];

            string driverDelegateType = string.Empty;

            switch (dbProviderName)
            {
                case "SQLite-Microsoft":
                case "SQLite":
                    driverDelegateType = typeof(SQLiteDelegate).AssemblyQualifiedName; break;
                case "MySql":
                    driverDelegateType = typeof(MySQLDelegate).AssemblyQualifiedName; break;
                case "OracleODPManaged":
                    driverDelegateType = typeof(OracleDelegate).AssemblyQualifiedName; break;
                case "SqlServer":
                case "SQLServerMOT":
                    driverDelegateType = typeof(SqlServerDelegate).AssemblyQualifiedName; break;
                case "Npgsql":
                    driverDelegateType = typeof(PostgreSQLDelegate).AssemblyQualifiedName; break;
                case "Firebird":
                    driverDelegateType = typeof(FirebirdDelegate).AssemblyQualifiedName; break;
                default:
                    throw new System.Exception("dbProviderName unreasonable");
            }
            JudgeConfigureConn(context);
            var dbcontext = context.Services.BuildServiceProvider().GetService<CafJobDbContext>();
            SchedulerCenter schedulerCenter = SchedulerCenter.Instance;
            schedulerCenter.Setting(new DbProvider(dbProviderName, connectionString), driverDelegateType, dbcontext);

            return schedulerCenter;
        }

        private void JudgeConfigureConn(CafConfigurationContext context)
        {
            //notice:使用方需要指定配置AppSettingsConnection
            string conn = context.Configuration.GetSection("Quartz")["connectionString"];
            try
            {
                //var dboptions = new DbContextOptionsBuilder<CafAppsettingDbContext>().UseSqlServer(conn); ;
                //context.Services.AddScoped(s => new CafAppsettingDbContext(dboptions.Options));
                context.Services.AddDbContext<CafJobDbContext>(options => options.UseSqlServer(conn));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
