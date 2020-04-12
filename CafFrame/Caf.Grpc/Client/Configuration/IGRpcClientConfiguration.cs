using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Caf.Grpc.Client.Configuration
{
    public interface IGrpcClientConfiguration
    {

        /// <summary>
        /// Debug 模式的 Grpc 服务端配置
        /// </summary>
        GrpcDirectConnectionConfiguration GrpcDirectConnectionConfiguration { get; set; }
        GrpcClientConfiguration AddRpcClientAssembly(Assembly serviceAssembly);
        List<Assembly> GrpcClientAssemblies { get; set; }
    }
}
