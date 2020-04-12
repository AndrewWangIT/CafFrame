using Caf.Core.DependencyInjection;
using Caf.Core.Module;
using Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;

namespace Caf.Job
{
    public class CafJobModule : CafModule
    {
        public override void ConfigureServices(CafConfigurationContext context)
        {
            context.Services.AddSingleton(GetScheduler(context.Configuration));
        }
        public override void OnApplicationInitialization(CafApplicationContext context)
        {
            //context.ServiceProvider.GetService<SchedulerCenter>()?.Init();//初始化Job
            var app = context.ServiceProvider.GetRequiredService<ObjectWrapper<IApplicationBuilder>>().Value;
            app.Use(async (context, next) =>
            {
                //await next();
                //context.Response.StatusCode >0 &&
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
        private SchedulerCenter GetScheduler(IConfiguration configuration)
        {
            string dbProviderName = configuration.GetSection("Quartz")["dbProviderName"];
            string connectionString = configuration.GetSection("Quartz")["connectionString"];

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

            SchedulerCenter schedulerCenter = SchedulerCenter.Instance;
            schedulerCenter.Setting(new DbProvider(dbProviderName, connectionString), driverDelegateType);

            return schedulerCenter;
        }
    }
}
