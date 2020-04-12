using Caf.Core.Module;
using Caf.DynamicWebApi;
using Caf.Grpc;
using Caf.Grpc.Client.Configuration;
using Caf.Grpc.Server;
using Caf.Grpc.Client;
using Capgemini.Frame.AspNetCore;
using Caf.Grpc.Server.Extensions;
using Caf.Grpc.Client.Extensions;
using Caf.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Capgemini.Caf;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using CafApi.GrpcService;
using Caf.Job;
using Newtonsoft.Json.Serialization;

namespace CafApi
{
    [UsingModule(typeof(CafJobModule))]
    [UsingModule(typeof(CafGrpcServerModule))]
    [UsingModule(typeof(CafAspNetCoreModule))]
    [UsingModule(typeof(DynamicWebApiModule))]

    public class sampleModule:CafModule
    {
        public override void BeforeConfigureServices(CafConfigurationContext context)
        {
            context.Services.AddSingleton<ITestNewService, TestNewService>();
            context.Services.AddSingleton<ITestService, TestService>();
            
            context.AddCafCors(o => { o.ConfigurationSection = "App:CorsOrigins"; o.Enable = true; });//ÃÌº”øÁ”Ú
            //context.Services.AddSingleton<ITestNewService, TestNewService>();
            context.UseGrpcService
                (
                o => {
                    o.GrpcBindAddress = "0.0.0.0";
                    o.GrpcBindPort = 8989;
                }).AddRpcServiceAssembly(typeof(sampleModule).Assembly);
            //context.UseGrpcClientForDirectConnection(new[]
            //{
            //    new GrpcServerNode
            //    {
            //        GrpcServiceIp = "127.0.0.1",
            //        GrpcServiceName = "TestServiceName",
            //        GrpcServicePort = 8989
            //    }
            //}).AddRpcClientAssembly(typeof(sampleModule).Assembly);
            context.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver() { NamingStrategy = new DefaultNamingStrategy() });
        }
        public override void OnApplicationInitialization(CafApplicationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCafExceptionHandler();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}