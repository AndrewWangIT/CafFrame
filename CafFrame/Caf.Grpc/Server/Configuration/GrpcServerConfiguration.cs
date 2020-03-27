using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Caf.Grpc.Server.Configuration
{
    public class GrpcServerConfiguration : IGrpcServerConfiguration
    {

        private readonly List<Assembly> _grpcAssemblies;
        public GrpcServerConfiguration()
        {
            _grpcAssemblies = new List<Assembly>();
        }
        public IReadOnlyList<Assembly> GrpcAssemblies => _grpcAssemblies;

        public string GrpcBindAddress { get; set; }

        /// <inheritdoc />
        public int GrpcBindPort { get; set; }

        public void AddRpcServiceAssembly(Assembly serviceAssembly)
        {
            _grpcAssemblies.Add(serviceAssembly);
        }
    }
}
