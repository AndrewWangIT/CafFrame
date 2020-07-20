using System;
namespace Cafgemini.Frame.Grpc.Server.DynamicGenerator
{
    public interface IGrpcServiceProvider
    {
         Type[] NeedProxyGrpcServiceType{get;}

        Type[] NeedProxyGrpcClientType { get; }
    }
}