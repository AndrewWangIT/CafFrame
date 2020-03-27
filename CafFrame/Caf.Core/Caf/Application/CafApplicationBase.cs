using System;
using System.Collections.Generic;
using System.Text;
using Caf.Core.DependencyInjection;
using Caf.Core.Module;
using Caf.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Caf.Core
{
    public abstract class CafApplicationBase : ICafApplication
    {
        public Type StartupModuleType{ get; }

        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }
        public IServiceProvider ServiceProvider { get; private set; }

        public IReadOnlyList<ICafModuleDescriptor> Modules { get; }

        internal CafApplicationBase(Type startupModuleType, IServiceCollection services, IConfiguration configuration,Action<CafApplicationCreationOptions> optionsAction)
        {
            StartupModuleType = startupModuleType;
            Services = services;
            services.AddObjectWrapper<IServiceProvider>();
            Configuration = configuration;
            var options = new CafApplicationCreationOptions(services);
            optionsAction?.Invoke(options);

            services.AddSingleton<ICafApplication>(this);
            services.AddSingleton<IModuleContainer>(this);

            services.AddCoreServices();
            services.AddCoreCafServices(options);

            Modules = LoadModules(services, configuration, options);
        }
        public virtual void Dispose()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                scope.ServiceProvider
                    .GetRequiredService<IModuleManager>()
                    .ShutdownModules(new CafApplicationContext(scope.ServiceProvider));
            }
        }

        public void Shutdown()
        {
            
        }
        protected virtual void SetServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ServiceProvider.GetRequiredService<ObjectWrapper<IServiceProvider>>().Value = ServiceProvider;
        }
        protected virtual void InitializeModules()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                scope.ServiceProvider
                    .GetRequiredService<IModuleManager>()
                    .InitializeModules(new CafApplicationContext(scope.ServiceProvider));
            }
        }
        private IReadOnlyList<ICafModuleDescriptor> LoadModules(IServiceCollection services, IConfiguration configuration, CafApplicationCreationOptions options)
        {
            return services
                .GetSingletonInstance<IModuleLoader>()
                .LoadModules(
                    services,
                    configuration,
                    StartupModuleType
                );
        }

    }
}
