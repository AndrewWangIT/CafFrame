using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Caf.Grpc.Client.Configuration
{
    public class GrpcClientConfiguration: IGrpcClientConfiguration
    {
        private List<Assembly> _grpcClientAssemblies;//Client 程序集
        /// <summary>
        /// 直连模式需要进行的配置
        /// </summary>
        public GrpcDirectConnectionConfiguration GrpcDirectConnectionConfiguration { get; set; }
        public List<Assembly> GrpcClientAssemblies { get => _grpcClientAssemblies; set => _grpcClientAssemblies = value; }
        public GrpcClientConfiguration()
        {
            GrpcDirectConnectionConfiguration = new GrpcDirectConnectionConfiguration();
            _grpcClientAssemblies = new List<Assembly>();
        }

        public GrpcClientConfiguration AddRpcClientAssembly(Assembly serviceAssembly)
        {
            _grpcClientAssemblies.Add(serviceAssembly);
            return this;
        }
    }
}
