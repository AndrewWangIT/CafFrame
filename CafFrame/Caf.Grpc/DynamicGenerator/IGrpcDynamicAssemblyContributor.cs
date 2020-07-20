using System;
using System.Reflection;

namespace Cafgemini.Frame.Grpc.Server.DynamicGenerator
{
    public interface IGrpcDynamicAssemblyContributor
    {
         Assembly GrpcDynamicAssembly{get;}
    }
}