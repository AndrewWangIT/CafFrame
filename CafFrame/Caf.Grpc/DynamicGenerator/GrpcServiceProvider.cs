using Caf.Grpc.Client.Configuration;
using Caf.Grpc.DynamicGenerator;
using Caf.Grpc.Server.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cafgemini.Frame.Grpc.Server.DynamicGenerator
{
    public class GrpcServiceProvider : IGrpcServiceProvider
    {
        private readonly IGrpcServerConfiguration _grpcServerConfiguration;
        private readonly IGrpcClientConfiguration _grpcClientConfiguration;
        public GrpcServiceProvider(IGrpcServerConfiguration grpcServerConfiguration, IGrpcClientConfiguration grpcClientConfiguration)
        {
            _grpcServerConfiguration = grpcServerConfiguration;
            _grpcClientConfiguration = grpcClientConfiguration;
            //if (grpcServerConfiguration.GrpcAssemblies == null || grpcServerConfiguration.GrpcAssemblies.Count == 0)
            //{
            //    throw new ArgumentNullException("GrpcAssemblies of grpcServerConfiguration can not be null ,are you missing AddRpcServiceAssembly ?");
            //}
            //var types = new List<Type>();
            //foreach (var assembly in grpcServerConfiguration.GrpcAssemblies)
            //{
            //    var interfaceType = assembly.GetTypes().Where(o => o.IsInterface && o.CustomAttributes.Any(c => c.AttributeType == typeof(MagicOnionGrpcAttribute)));
            //    types.AddIfNotContains(interfaceType);
            //}
            //_needProxyTypes = types.ToArray();

            //var clientTypes = new List<Type>();
            //foreach (var assembly in grpcClientConfiguration.GrpcClientAssemblies)
            //{
            //    var interfaceType = assembly.GetTypes().Where(o => o.IsInterface && o.CustomAttributes.Any(c => c.AttributeType == typeof(MagicOnionGrpcAttribute)));
            //    clientTypes.AddIfNotContains(interfaceType);
            //}
            //_needProxyClientInterfaceTypes = clientTypes.ToArray();
        }
        private Type[] _needProxyTypes;//需要被动态生成的接口
        private Type[] _needProxyClientInterfaceTypes;//需要被动态生成的客户端接口
        public Type[] NeedProxyGrpcClientType
        {
            get
            {
                var clientTypes = new List<Type>();
                foreach (var assembly in _grpcClientConfiguration.GrpcClientAssemblies)
                {
                    var interfaceType = assembly.GetTypes().Where(o => o.IsInterface && o.CustomAttributes.Any(c => c.AttributeType == typeof(MagicOnionGrpcAttribute)));
                    clientTypes.AddIfNotContains(interfaceType);
                }
                _needProxyClientInterfaceTypes = clientTypes.ToArray();
                return _needProxyClientInterfaceTypes;
            }        
        }


        //public Type[] NeedProxyGrpcServiceType => {};
        public Type[] NeedProxyGrpcServiceType
        {
            get
            {
                if (_grpcServerConfiguration.GrpcAssemblies == null || _grpcServerConfiguration.GrpcAssemblies.Count == 0)
                {
                    throw new ArgumentNullException("GrpcAssemblies of grpcServerConfiguration can not be null ,are you missing AddRpcServiceAssembly ?");
                }
                var types = new List<Type>();
                foreach (var assembly in _grpcServerConfiguration.GrpcAssemblies)
                {
                    var interfaceType = assembly.GetTypes().Where(o => o.IsInterface && o.CustomAttributes.Any(c => c.AttributeType == typeof(MagicOnionGrpcAttribute)));
                    types.AddIfNotContains(interfaceType);
                }
                _needProxyTypes = types.ToArray();
                return _needProxyTypes;
            }
        }
    }
}