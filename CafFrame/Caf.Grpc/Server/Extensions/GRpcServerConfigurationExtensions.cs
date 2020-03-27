using Caf.Core.Module;
using Caf.Grpc.Server.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
namespace Caf.Grpc.Server.Extensions
{
    public static class GRpcServerConfigurationExtensions
    {
        public static IGrpcServerConfiguration UseGrpcService(this CafConfigurationContext context, Action<IGrpcConfiguration> optionAction)
        {
            var config = context.Services.GetSingletonInstance<IGrpcServerConfiguration>();
            optionAction(config);
            return config;
        }
    }
}
