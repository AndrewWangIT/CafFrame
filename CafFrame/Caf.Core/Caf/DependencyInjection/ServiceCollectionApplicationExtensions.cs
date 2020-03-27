using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using AspectCore.Injector;
using Caf.Core.DynamicProxy;
using Caf.Core.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.DependencyInjection
{
    public static class ServiceCollectionApplicationExtensions
    {
        public static ICafApplicationServiceProvider ConfigureModule<TStartupModule>(
             this IServiceCollection services,
             IConfiguration configuration,
             Action<CafApplicationCreationOptions> optionsAction = null)
            where TStartupModule : ICafModule
        {
            return CafApplicationFactory.Create<TStartupModule>(services, configuration, optionsAction);
        }

        public static ICafApplicationServiceProvider AddApplication(
             this IServiceCollection services,
             IConfiguration configuration,
             Type startupModuleType,
             Action<CafApplicationCreationOptions> optionsAction = null)
        {
            return CafApplicationFactory.Create(startupModuleType, services, configuration, optionsAction);
        }
        public static void BuildServiceProviderFromFactory(this IServiceContainer services, IServiceCollection serviceCollection)
        {
            var interceptorContexts = serviceCollection.GetInterceptorContextList();
            services.Configure(config =>
            {
                foreach (var item in interceptorContexts)
                {
                    config.Interceptors.AddTyped(item.TInterceptor, item.AspectPredicates);
                }
                
            });          

        }
    }
}
