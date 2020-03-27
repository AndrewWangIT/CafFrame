using Caf.Core.Module;
using Caf.Grpc.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Caf.Core;

namespace Caf.Grpc.Client.Extensions
{
    public static class GrpcClientConfigurationExtensions
    {
        /// <summary>
        /// 启用直连模式的 Grpc Client 客户端连接
        /// </summary>
        /// <param name="configs"></param>
        /// <param name="grpcNodes">Grpc 服务器节点列表</param>
        public static void UseGrpcClientForDirectConnection(this CafConfigurationContext context, params GrpcServerNode[] grpcNodes)
        {
            var internalDict = context.Services.GetSingletonInstance<IGrpcClientConfiguration>().GrpcDirectConnectionConfiguration.GrpcServerNodes;

            foreach (var grpcNode in grpcNodes)
            {
                if (internalDict.ContainsKey(grpcNode.GrpcServiceName))
                {
                    throw new CafException("不能添加重复的名称的 Grpc 服务节点.");
                }

                internalDict.Add(grpcNode.GrpcServiceName, grpcNode);
            }
        }
    }
}
