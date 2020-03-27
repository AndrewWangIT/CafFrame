using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Caf.Grpc.Server.Configuration
{
    public interface IGrpcServerConfiguration : IGrpcConfiguration
    {
        /// <summary>
        /// 存在 Grpc 服务的程序集集合
        /// </summary>
        IReadOnlyList<Assembly> GrpcAssemblies { get; }

        /// <summary>
        /// 添加包含 Grpc 服务定义的程序集
        /// </summary>
        /// <param name="serviceAssembly">服务程序集</param>
        void AddRpcServiceAssembly(Assembly serviceAssembly);
    }
}
