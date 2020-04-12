using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Caf.Grpc.Server.Configuration
{
    public class GrpcServerConfiguration : IGrpcServerConfiguration
    {

        private List<Assembly> _grpcAssemblies;
        public GrpcServerConfiguration()
        {
            _grpcAssemblies = new List<Assembly>();
        }

        public string GrpcBindAddress { get; set; }

        /// <inheritdoc />
        public int GrpcBindPort { get; set; }
        public List<Assembly> GrpcAssemblies { get => _grpcAssemblies; set => _grpcAssemblies = value; }

        public GrpcServerConfiguration AddRpcServiceAssembly(Assembly serviceAssembly)
        {
            _grpcAssemblies.Add(serviceAssembly);
            return this;
        }
    }
}
