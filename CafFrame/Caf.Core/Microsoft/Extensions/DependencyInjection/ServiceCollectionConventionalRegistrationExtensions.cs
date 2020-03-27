using Caf.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceAutoRegistExtensions
    {

        public static IServiceCollection AddAssemblyOf<T>(this IServiceCollection services)
        {
            return services.AddAssembly(typeof(T).GetTypeInfo().Assembly);
        }

        public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly assembly)
        {
            var registrar = services.GetSingletonInstance<IDefaultAutoRegistrar>();
            registrar.AddAssembly(services, assembly);
            return services;
        }

        public static IServiceCollection AddTypes(this IServiceCollection services, params Type[] types)
        {
            var registrar = services.GetSingletonInstance<IDefaultAutoRegistrar>();
            registrar.AddTypes(services, types);
            return services;
        }

        public static IServiceCollection AddType<TType>(this IServiceCollection services)
        {
            return services.AddType(typeof(TType));
        }

        public static IServiceCollection AddType(this IServiceCollection services, Type type)
        {
            var registrar = services.GetSingletonInstance<IDefaultAutoRegistrar>();
            registrar.AddType(services, type);
            return services;
        }
    }
}
