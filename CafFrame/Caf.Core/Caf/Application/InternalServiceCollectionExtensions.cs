using Caf.Core.DependencyInjection;
using Caf.Core.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Caf.Core
{
    internal static class InternalServiceCollectionExtensions
    {
        internal static void AddCoreServices(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging();
        }

        internal static void AddCoreCafServices(this IServiceCollection services,
            CafApplicationCreationOptions applicationCreationOptions)
        {
            var moduleLoader = new ModuleLoader();

            if (!services.IsAdded<IConfiguration>())
            {
                services.AddSingleton<IConfiguration>(ConfigurationHelper.BuildConfiguration(
                    applicationCreationOptions.Configuration
                ));
            }

            services.TryAddSingleton<IModuleLoader>(moduleLoader);
            services.TryAddSingleton<IDefaultAutoRegistrar>(new DefaultAutoRegistrar());
            services.AddAssemblyOf<ICafApplication>();
        }
    }
}
