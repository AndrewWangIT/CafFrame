using Caf.AspNetCore;
using Caf.Core.Module;
using Caf.Grpc.Client;
using Caf.Grpc.Client.Configuration;
using Caf.Grpc.Client.Extensions;
using Caf.Grpc.DynamicGenerator;
using Capgemini.Caf;
using Capgemini.Frame.AspNetCore;
using MessagePack;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace grpcClientTest
{
    [UsingModule(typeof(CafGrpcClientModule))]
    [UsingModule(typeof(CafAspNetCoreModule))]
    public class GrpcTestMoudle : CafModule
    {
        public override void BeforeConfigureServices(CafConfigurationContext context)
        {
            context.AddCafCors(o => { o.ConfigurationSection = "App:CorsOrigins"; o.Enable = true; });//添加跨域
            context.UseGrpcClientForDirectConnection(new[]
            {
                new GrpcServerNode
                {
                    GrpcServiceIp = "127.0.0.1",
                    GrpcServiceName = "TestServiceName",
                    GrpcServicePort = 8989
                }
            }).AddRpcClientAssembly(typeof(GrpcTestMoudle).Assembly);
            context.Services.AddHttpClient("webapi", c =>
            {
                c.BaseAddress = new Uri("http://localhost:44300/");
            });
            context.Services.AddControllers();
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
    [MagicOnionGrpc]
    public interface ITestNewService
    {
        Task<TestData> SayHello(string name);
    }
    [MessagePackObject]
    public class TestData
    {
        [Key(0)]
        public string aa = "";
        [Key(1)]
        public long bb = 1;
        [Key(2)]
        public List<string> cc = new List<string> { };
    }
}
