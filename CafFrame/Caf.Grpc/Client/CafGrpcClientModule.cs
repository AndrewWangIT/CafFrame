using Caf.Core.Module;
using Caf.Grpc.Client.Configuration;
using Caf.Grpc.Client.Utility;
using Caf.Grpc.Server.Configuration;
using Cafgemini.Frame.Grpc.Server.DynamicGenerator;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Grpc.Client
{
    public class CafGrpcClientModule: CafModule
    {
        public override void BeforeConfigureServices(CafConfigurationContext context)
        {
            if (context.Services.GetSingletonInstanceOrNull<IGrpcServerConfiguration>()==null)
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
        public override void AfterConfigureServices(CafConfigurationContext context)
        {
            context.Services.AddSingleton<IGrpcConnectionUtility, GRpcConnectionUtility>();
            var generator = context.Services.GetSingletonInstance<IGrpcServiceGenerator>();
            List<Type> types;
            List<Type> proxyTypes;
            generator.GeneraterClientProxyService(out types,out proxyTypes);
            foreach (var type in types)
            {
                var implType = proxyTypes.Find(o => type.IsAssignableFrom(o));
                if(implType!=null)
                {
                    context.Services.AddSingleton(type, implType);
                }
            }
        }
        public override void OnApplicationInitialization(CafApplicationContext context)
        {

        }
        public override void OnApplicationShutdown(CafApplicationContext context)
        {
            var grpcConfig = context.ServiceProvider.GetRequiredService<IGrpcClientConfiguration>();

            foreach (var serverNodes in grpcConfig.GrpcDirectConnectionConfiguration.GrpcServerNodes)
            {
                serverNodes.Value?.InternalChannel?.ShutdownAsync();
            } 
        }
    }
}
