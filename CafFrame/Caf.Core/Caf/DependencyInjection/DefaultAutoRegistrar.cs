using Caf.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Caf.Core.DependencyInjection
{
    public class DefaultAutoRegistrar: IDefaultAutoRegistrar
    {
        public void AddType(IServiceCollection services, Type type)
        {
            if (IsAutoRegistrationDisabled(type))
            {
                return;
            }
            var lifeTime = GetDefaultNeedRegisServiceLifetime(type);

            if (lifeTime == null)
            {
                return;
            }
            foreach (var serviceDescriptor in GetDefaultServiceDescriptor(type, lifeTime.Value))
            {
                services.Add(serviceDescriptor);
            }
        }
        /// <summary>
        /// 获取默认的注入接口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        private List<ServiceDescriptor> GetDefaultServiceDescriptor(Type type, ServiceLifetime serviceLifetime)
        {
            //注入自生
            var serviceTypes = new List<Type>() { type };

            foreach (var interfaceType in type.GetTypeInfo().GetInterfaces())
            {
                var interfaceName = interfaceType.Name;

                //if (interfaceName.StartsWith("I"))
                //{
                //    interfaceName = interfaceName.Right(interfaceName.Length - 1);
                //}

                if (type.Name!= "ISingleton" && type.Name != "IScoped" && type.Name != "ITransient")//.EndsWith(interfaceName)
                {
                    serviceTypes.Add(interfaceType);
                }
            }
            return serviceTypes.Select(serviceType => ServiceDescriptor.Describe(serviceType, type, serviceLifetime)).ToList();
        }

        protected virtual ServiceLifetime? GetDefaultNeedRegisServiceLifetime(Type type)
        {
            if (typeof(ITransient).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Transient;
            }

            if (typeof(ISingleton).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Singleton;
            }

            if (typeof(IScoped).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Scoped;
            }

            return null;
        }

        public virtual void AddAssembly(IServiceCollection services, Assembly assembly)
        {
            var types = AssemblyHelper
                .GetAllTypes(assembly)
                .Where(
                    type => type != null &&
                            type.IsClass &&
                            !type.IsAbstract &&
                            !type.IsGenericType
                ).ToArray();

            AddTypes(services, types);
        }

        public void AddTypes(IServiceCollection services, params Type[] types)
        {
            foreach (var type in types)
            {
                AddType(services, type);
            }
        }

        protected bool IsAutoRegistrationDisabled(Type type)
        {
            return type.IsDefined(typeof(DisableAutoRegistrationAttribute), true);
        }
    }
}
