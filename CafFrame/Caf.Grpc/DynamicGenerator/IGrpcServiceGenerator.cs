using System.Reflection;
using System;
using System.Collections.Generic;

namespace Capgemini.Frame.Grpc.Server.DynamicGenerator
{
    public interface IGrpcServiceGenerator
    {
        Type[] GeneraterProxyService();
        void GeneraterClientProxyService(out List<Type> types, out List<Type> proxyTypes);
        List<Tuple<Type, Type>> GeneraterClientProxyInterface();
        Assembly DynamicAssembly{get;}
        Assembly DynamicInterfaceAssembly { get; }
        Assembly DynamicClientInterfaceAssembly { get; }
    }
}