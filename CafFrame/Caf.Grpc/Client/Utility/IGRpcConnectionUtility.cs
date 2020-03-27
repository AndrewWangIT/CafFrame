using MagicOnion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Grpc.Client.Utility
{
    public interface IGrpcConnectionUtility
    {

        /// <summary>
        /// 使用缓存的指定服务 Channel 访问 Grpc 接口，出现异常时会抛出错误信息
        /// </summary>
        /// <typeparam name="TService">远程接口服务类型</typeparam>
        /// <param name="serviceName">远程服务名称</param>
        /// <returns></returns>
        TService GetRemoteServiceForDirectConnection<TService>(string serviceName) where TService : IService<TService>;
    }
}
