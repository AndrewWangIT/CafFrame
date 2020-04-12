using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Caf.Core;
using Caf.Grpc.Client.Configuration;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;

namespace Caf.Grpc.Client.Utility
{
    public class GRpcConnectionUtility : IGrpcConnectionUtility
    {
        public IGrpcClientConfiguration GrpcClientConfiguration { get; set; }
        public Dictionary<Type, Type> TypeMapping { get; set; }//记录原先接口类，与生成的Grpc接口类的映射关系
        MethodInfo ClientCreate;
        public GRpcConnectionUtility(IGrpcClientConfiguration grpcClientConfiguration)
        {
            ClientCreate = typeof(MagicOnionClient).GetMethod("Create", 1, new Type[] { typeof(Channel) });
            GrpcClientConfiguration = grpcClientConfiguration;
        }
        public TService GetRemoteServiceForDirectConnection<TService>(string serviceName) where TService : IService<TService>
        {
            var nodeInfo = GrpcClientConfiguration.GrpcDirectConnectionConfiguration[serviceName];

            if (nodeInfo == null) throw new CafException("Grpc 服务没有在启动时定义，或者初始化内部 Channel 失败.");

            if (nodeInfo.InternalChannel != null)
            {
                //if (typeof(IServiceMarker).IsAssignableFrom(typeof(TService)))
                //{
                //    //ClientCreate.MakeGenericMethod(new Type[] { typeof(TService) });
                //    return (TService)ClientCreate.MakeGenericMethod(new Type[] { typeof(TService) }).Invoke(null, new object[] { nodeInfo.InternalChannel }); //MagicOnionClient.Create<TService>(nodeInfo.InternalChannel);
                //}
                //else
                //{
                //    if(TypeMapping.ContainsKey(typeof(TService)))
                //    {
                //        return (TService)ClientCreate.MakeGenericMethod(new Type[] { TypeMapping[typeof(TService)] }).Invoke(null, new object[] { nodeInfo.InternalChannel });
                //    }
                //    //ClientCreate.MakeGenericMethod(new Type[] { typeof(TService) });
                //}
                return MagicOnionClient.Create<TService>(nodeInfo.InternalChannel);
            }

            throw new CafException("无法创建 Grpc 服务.");
        }
    }


}
