using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caf.Core.Module;
using Caf.Grpc.Server.Configuration;
using Caf.Grpc.Server.DependencyInject;
using Caf.Grpc.Server.Extensions;
using Caf.Grpc.Server.HostService;
using Grpc.Core;
using MagicOnion.Hosting;
using MagicOnion.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Caf.Grpc.Server
{
    public class CafGrpcServerModule: CafModule
    {
        public override void BeforeConfigureServices(CafConfigurationContext context)
        {
            context.Services.AddSingleton<IGrpcServerConfiguration>(new GrpcServerConfiguration());
        }
        public override void ConfigureServices(CafConfigurationContext context)
        {
            //context.UseGrpcService();
        }

        public override void AfterConfigureServices(CafConfigurationContext context)
        {
            //context.Services.AddMagicOnionHostedServiceDefinition();
            PostConfigureGrpcServer(context);
        }

        public override void OnApplicationInitialization(CafApplicationContext context)
        {
            var options = new MagicOnionOptions();
            options.IsReturnExceptionStackTraceInErrorDetail = true;
            var factory = context.ServiceProvider.GetService<IServiceScopeFactory>();
            options.ServiceLocator = new MicrosoftExtensionsServiceLocator(factory.CreateScope().ServiceProvider, options);//new ServiceLocatorBridge(factory);
            //var aaa = context.ServiceProvider.GetService<IServiceScopeFactory>();
            options.MagicOnionServiceActivator = new MicrosoftExtensionsMagicOnionServiceActivator();
            MagicOnionServiceDefinition serviceDefine = null;
            var config = context.ServiceProvider.GetRequiredService<IGrpcServerConfiguration>();
            if (config.GrpcAssemblies != null)
            {
                serviceDefine = MagicOnionEngine.BuildServerServiceDefinition(config.GrpcAssemblies.ToArray(), options);
            }
            var packageMagicOnionServerService = new PackageMagicOnionServerService(serviceDefine, new[]
            {
                new ServerPort(config.GrpcBindAddress, config.GrpcBindPort, ServerCredentials.Insecure)
            }, null);
            packageMagicOnionServerService.StartAsync(CancellationToken.None);

            //AsyncHelper.RunSync(() => context.ServiceProvider.GetRequiredService<IHostedService>().StartAsync(CancellationToken.None));
        }
        /// <summary>
        /// 初始化 Grpc 服务
        /// </summary>
        /// <param name="config">Grpc 配置项</param>
        private void PostConfigureGrpcServer(CafConfigurationContext context)
        {
            //var options = new MagicOnionOptions();
            //options.IsReturnExceptionStackTraceInErrorDetail = true;
            ////options.ServiceLocator = new ServiceLocatorBridge(context.);

            //// 构建 gRpc 服务。
            //MagicOnionServiceDefinition serviceDefine = null;
            //var config = context.Services.GetSingletonInstance<IGrpcServerConfiguration>();
            //if (config.GrpcAssemblies != null)
            //{
            //    serviceDefine = MagicOnionEngine.BuildServerServiceDefinition(config.GrpcAssemblies.ToArray(), new MagicOnionOptions() { IsReturnExceptionStackTraceInErrorDetail=true });
            //}
            // 注入 gRpc 服务到 IoC 容器当中。
            //context.Services.AddSingleton<MagicOnionServiceDefinition>(serviceDefine);
            //context.Services.AddSingleton<PackageMagicOnionServerService>(new PackageMagicOnionServerService(serviceDefine, new[]
            //{
            //    new ServerPort(config.GrpcBindAddress, config.GrpcBindPort, ServerCredentials.Insecure)
            //}, null));
            //context.Services.AddHostedService<PackageMagicOnionServerService>(o=>o.GetRequiredService<PackageMagicOnionServerService>());
        }
    }
}
