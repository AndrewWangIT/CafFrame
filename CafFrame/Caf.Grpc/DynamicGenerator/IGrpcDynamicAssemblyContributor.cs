using System;
using System.Reflection;

namespace Capgemini.Frame.Grpc.Server.DynamicGenerator
{
    public interface IGrpcDynamicAssemblyContributor
    {
         Assembly GrpcDynamicAssembly{get;}
    }
}