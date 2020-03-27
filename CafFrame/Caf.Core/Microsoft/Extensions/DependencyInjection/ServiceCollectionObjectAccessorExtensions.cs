using Caf.Core;
using Caf.Core.DependencyInjection;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionObjectWrapperExtensions
    {
        public static ObjectWrapper<T> AddObjectWrapper<T>(this IServiceCollection services)
        {
            if (services.Any(o => o.ServiceType == typeof(ObjectWrapper<T>)))
            {
                return services.GetSingletonInstance<ObjectWrapper<T>>();
            }
            return services.AddObjectWrapper(new ObjectWrapper<T>());
        }

        public static ObjectWrapper<T> AddObjectWrapper<T>(this IServiceCollection services, T obj)
        {
            return services.AddObjectWrapper(new ObjectWrapper<T>(obj));
        }

        public static T GetObjectOrNull<T>(this IServiceCollection services)where T : class
        {
            return services.GetSingletonInstanceOrNull<IObjectWrapper<T>>()?.Value;
        }
        public static ObjectWrapper<T> AddObjectWrapper<T>(this IServiceCollection services, ObjectWrapper<T> wrapper)
        {
            services.AddSingleton(typeof(ObjectWrapper<T>), wrapper);
            services.AddSingleton(typeof(IObjectWrapper<T>), wrapper);
            return wrapper;
        }
    }
}