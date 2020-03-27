using AspectCore.Configuration;
using Caf.Core.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Caf.UnitOfWork
{
    public static class UowRegistar
    {
        public static InterceptorContext RegisterIfNeeded(IServiceCollection Services)
        {
            //后续优化，反射过多，启动时间可能变长
            AspectPredicate lamda = method =>
            {
                var ImplementationType = Services.Where(o => o.ServiceType == method.DeclaringType).FirstOrDefault()?.ImplementationType;
                MethodInfo ImplementationMethod = null;
                try
                {
                    var methods = ImplementationType?.GetMethods().Where(o => o.Name == method.Name && o.IsGenericMethod == method.IsGenericMethod) ?? new List<MethodInfo>();
                    foreach (var item in methods)
                    {
                        if (string.Join(',', item.GetParameters().Select(o => o.ParameterType.FullName)) == string.Join(',', method.GetParameters().Select(o => o.ParameterType.FullName)))
                        {
                            ImplementationMethod = item;
                        }
                    }
                }
                catch
                {
                    return false;
                }

                return method.IsDefined(typeof(UowAttribute), true) || (ImplementationMethod?.IsDefined(typeof(UowAttribute), true) ?? false) || ShouldUowByDefault(method.DeclaringType) || ShouldUowByDefault(ImplementationType);
            };
            return new InterceptorContext
            {
                TInterceptor = typeof(UowInterceptor),
                AspectPredicates = new AspectPredicate[] { lamda }

            };
        }

        public static bool ShouldUowByDefault(Type type)
        {
            if (type == null)
            {
                return false;
            }
            if (type.IsDefined(typeof(UowAttribute), true))
            {
                return true;
            }

            return false;
        }
    }
}
