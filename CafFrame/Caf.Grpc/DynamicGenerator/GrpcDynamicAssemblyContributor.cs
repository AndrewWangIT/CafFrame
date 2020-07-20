using System.Reflection;

namespace Cafgemini.Frame.Grpc.Server.DynamicGenerator
{
    public class GrpcDynamicAssemblyContributor : IGrpcDynamicAssemblyContributor
    {
        private readonly Assembly _grpcDynamicAssembly;
        public GrpcDynamicAssemblyContributor()
        {

        }

        public Assembly GrpcDynamicAssembly => _grpcDynamicAssembly;
    }
}