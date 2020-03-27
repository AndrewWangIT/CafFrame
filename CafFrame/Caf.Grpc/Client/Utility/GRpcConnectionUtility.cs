using System;
using System.Collections.Generic;
using System.Text;
using Caf.Core;
using Caf.Grpc.Client.Configuration;
using MagicOnion;
using MagicOnion.Client;

namespace Caf.Grpc.Client.Utility
{
    public class GRpcConnectionUtility : IGrpcConnectionUtility
    {
        public IGrpcClientConfiguration GrpcClientConfiguration { get; set; }
        public GRpcConnectionUtility(IGrpcClientConfiguration grpcClientConfiguration)
        {
            GrpcClientConfiguration = grpcClientConfiguration;
        }
        public TService GetRemoteServiceForDirectConnection<TService>(string serviceName) where TService : IService<TService>
        {
            var nodeInfo = GrpcClientConfiguration.GrpcDirectConnectionConfiguration[serviceName];

            if (nodeInfo == null) throw new CafException("Grpc 服务没有在启动时定义，或者初始化内部 Channel 失败.");

            if (nodeInfo.InternalChannel != null)
            {
                return MagicOnionClient.Create<TService>(nodeInfo.InternalChannel);
            }

            throw new CafException("无法创建 Grpc 服务.");
        }
    }
}
