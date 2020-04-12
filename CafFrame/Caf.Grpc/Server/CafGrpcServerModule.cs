using System.Linq;
using System.Threading;
using Caf.Core.Module;
using Caf.Grpc.Client.Configuration;
using Caf.Grpc.Server.Configuration;
using Caf.Grpc.Server.HostService;
using Capgemini.Frame.Grpc.Server.DynamicGenerator;
using Grpc.Core;
using MagicOnion.Hosting;
using MagicOnion.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Caf.Grpc.Server
{
    public class CafGrpcServerModule: CafModule
    {
        public override void BeforeConfigureServices(CafConfigurationContext context)
        {
            //context.Services.AddSingleton<IGrpcServerConfiguration>(new GrpcServerConfiguration());
            //context.Services.AddSingleton(typeof(IGrpcServiceProvider), typeof(GrpcServiceProvider));
            //context.Services.AddSingleton(typeof(IGrpcServiceGenerator), typeof(GrpcServiceGenerator));
            if (context.Services.GetSingletonInstanceOrNull<IGrpcServerConfiguration>() == null)
            {
                var grpcServerConfiguration = new GrpcServerConfiguration();
                var grpcClientConfiguration = new GrpcClientConfiguration();
                var grpcServiceProvider = new GrpcServiceProvider(grpcServerConfiguration, grpcClientConfiguration);
                context.Services.AddSingleton<IGrpcServerConfiguration>(grpcServerConfiguration);
                context.Services.AddSingleton(typeof(IGrpcClientConfiguration), grpcClientConfiguration);
                context.Services.AddSingleton(typeof(IGrpcServiceProvider), grpcServiceProvider);
                context.Services.AddSingleton(typeof(IGrpcServiceGenerator), new GrpcServiceGenerator(grpcServiceProvider));
            }
        }

        public override void OnApplicationInitialization(CafApplicationContext context)
        {
            var options = new MagicOnionOptions();
            options.IsReturnExceptionStackTraceInErrorDetail = true;
            options.ServiceLocator = new MicrosoftExtensionsServiceLocator(context.ServiceProvider, options);//new ServiceLocatorBridge(factory);
            options.MagicOnionServiceActivator = new MicrosoftExtensionsMagicOnionServiceActivator();
            MagicOnionServiceDefinition serviceDefine = null;
            var config = context.ServiceProvider.GetRequiredService<IGrpcServerConfiguration>();
            var generator = context.ServiceProvider.GetRequiredService<IGrpcServiceGenerator>();
            generator.GeneraterProxyService();//创建MagicOnion grpc 代理程序集
            config.GrpcAssemblies= config.GrpcAssemblies.Append(generator.DynamicAssembly).ToList();
            config.GrpcAssemblies = config.GrpcAssemblies.Append(generator.DynamicInterfaceAssembly).ToList();
            if (config.GrpcAssemblies != null)
            {
                serviceDefine = MagicOnionEngine.BuildServerServiceDefinition(config.GrpcAssemblies.ToArray(), options);
            }
            var packageMagicOnionServerService = new PackageMagicOnionServerService(serviceDefine, new[]
            {
                new ServerPort(config.GrpcBindAddress, config.GrpcBindPort, ServerCredentials.Insecure)
            }, null);
            packageMagicOnionServerService.StartAsync(CancellationToken.None);
        }
    }
}
