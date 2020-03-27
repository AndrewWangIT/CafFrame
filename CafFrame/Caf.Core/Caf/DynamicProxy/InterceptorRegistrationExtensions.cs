using Caf.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.DynamicProxy
{
    public static class InterceptorRegistrationExtensions
    {
        public static void OnRegistred(this IServiceCollection services, InterceptorContext interceptorContext)
        {
            GetOrCreateInterceptorContextList(services).Add(interceptorContext);
        }
        public static InterceptorContextList GetInterceptorContextList(this IServiceCollection services)
        {
            return GetOrCreateInterceptorContextList(services);
        }
        private static InterceptorContextList GetOrCreateInterceptorContextList(IServiceCollection services)
        {
            var contextList = services.GetSingletonInstanceOrNull<IObjectWrapper<InterceptorContextList>>()?.Value;
            if (contextList == null)
            {
                contextList = new InterceptorContextList();
                services.AddObjectWrapper(contextList);
            }

            return contextList;
        }
    }
}
