using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core
{
    public class CafApplicationServiceProvider : CafApplicationBase, ICafApplicationServiceProvider
    {
        public CafApplicationServiceProvider(
            Type startupModuleType,
            IServiceCollection services,
            IConfiguration configuration,
            Action<CafApplicationCreationOptions> optionsAction
            ) : base(
                startupModuleType,
                services,
                configuration,
                optionsAction)
        {
            services.AddSingleton<ICafApplicationServiceProvider>(this);
        }
        public void Run(IServiceProvider serviceProvider)
        {
            SetServiceProvider(serviceProvider);

            InitializeModules();
        }
    }
}
