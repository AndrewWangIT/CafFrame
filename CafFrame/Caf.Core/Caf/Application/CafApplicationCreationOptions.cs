using Caf.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core
{
    public class CafApplicationCreationOptions
    {
        public IServiceCollection Services { get; }

        public ConfigurationBuilderOptions Configuration { get; }

        public CafApplicationCreationOptions(IServiceCollection services)
        {
            Services = Check.NotNull(services, nameof(services));
            Configuration = new ConfigurationBuilderOptions();
        }
    }
}
