using Caf.Core.DependencyInjection;
using Caf.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caf.Core.Module
{
    public class ModuleManager : IModuleManager, ISingleton
    {
        private readonly IModuleContainer _moduleContainer;
        private readonly ILogger<ModuleManager> _logger;
        public ModuleManager(IModuleContainer moduleContainer, ILogger<ModuleManager> logger)
        {
            _moduleContainer = moduleContainer;
            _logger = logger;
        }
        public void InitializeModules(CafApplicationContext context)
        {
            foreach (var module in _moduleContainer.Modules)
            {
                ((IApplicationInitialization)module.Instance).OnPreApplicationInitialization(context);
            }
            foreach (var module in _moduleContainer.Modules)
            {
                ((IApplicationInitialization)module.Instance).OnApplicationInitialization(context);
            }
            foreach (var module in _moduleContainer.Modules)
            {
                ((IApplicationInitialization)module.Instance).OnPostApplicationInitialization(context);
            }
        }
        public void ShutdownModules(CafApplicationContext context)
        {
            var modules = _moduleContainer.Modules.Reverse().ToList();
            foreach (var module in modules)
            {
                ((IOnApplicationShutdown)module.Instance).OnApplicationShutdown(context);
            }
        }

       
    }
}
