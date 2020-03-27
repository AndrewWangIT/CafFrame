using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Caf.Core.Module
{
    public interface IModuleLoader
    {
        ICafModuleDescriptor[] LoadModules(
            IServiceCollection services,
            IConfiguration Configuration,
            Type startupModuleType
        );
    }
}
