using Caf.Core.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core
{
    public static class CafApplicationFactory
    {
        public static ICafApplicationServiceProvider Create<TStartupModule>(
            IServiceCollection services,
            IConfiguration configuration,
             Action<CafApplicationCreationOptions> optionsAction = null)
            where TStartupModule : ICafModule
        {
            return Create(typeof(TStartupModule), services, configuration,optionsAction);
        }

        public static ICafApplicationServiceProvider Create(
             Type startupModuleType,
             IServiceCollection services,
             IConfiguration configuration,
             Action<CafApplicationCreationOptions> optionsAction = null)
        {
            return new CafApplicationServiceProvider(startupModuleType, services, configuration, optionsAction);
        }
    }
}
