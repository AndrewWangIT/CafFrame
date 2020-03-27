using Caf.Core.Module;
using Caf.Grpc.Client.Configuration;
using Caf.Grpc.Client.Utility;
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
            context.Services.AddSingleton(typeof(IGrpcClientConfiguration),new GrpcClientConfiguration());
        }
        public override void AfterConfigureServices(CafConfigurationContext context)
        {
            context.Services.AddSingleton<IGrpcConnectionUtility, GRpcConnectionUtility>();
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
