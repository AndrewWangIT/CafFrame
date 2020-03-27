using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caf.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Caf.Core.Module
{
    public class ModuleLoader : IModuleLoader
    {
        public ICafModuleDescriptor[] LoadModules(IServiceCollection services, IConfiguration configuration, Type startupModuleType)
        {
            var modules = GetDescriptors(services, startupModuleType);
            modules = SortByDependency(modules, startupModuleType);
            ConfigureServices(modules, services, configuration);

            return modules.ToArray();
        }

        protected virtual void FillModules(
            List<CafModuleDescriptor> modules,
            IServiceCollection services,
            Type startupModuleType)
        {
            foreach (var moduleType in CafModuleHelper.FindAllModuleTyps(startupModuleType))
            {
                modules.Add(CreateModuleDescriptor(services, moduleType));
            }
        }
        protected virtual void SetDependencies(List<CafModuleDescriptor> modules)
        {
            foreach (var module in modules)
            {
                SetDependencies(modules, module);
            }
        }
        private List<ICafModuleDescriptor> GetDescriptors(
        IServiceCollection services,
        Type startupModuleType)
        {
            var modules = new List<CafModuleDescriptor>();

            FillModules(modules, services, startupModuleType);
            SetDependencies(modules);

            return modules.Cast<ICafModuleDescriptor>().ToList();
        }
        protected virtual List<ICafModuleDescriptor> SortByDependency(List<ICafModuleDescriptor> modules, Type startupModuleType)
        {
            var sortedModules = SortByDependencies(modules,m => m.Dependencies);
            sortedModules.MoveItem(m => m.Type == startupModuleType, modules.Count - 1);
            return sortedModules;
        }
        protected virtual ICafModule CreateAndRegisterModule(IServiceCollection services, Type moduleType)
        {
            var module = (ICafModule)Activator.CreateInstance(moduleType);
            services.AddSingleton(moduleType, module);
            return module;
        }
        protected virtual CafModuleDescriptor CreateModuleDescriptor(IServiceCollection services, Type moduleType, bool isLoadedAsPlugIn = false)
        {
            return new CafModuleDescriptor(moduleType, CreateAndRegisterModule(services, moduleType), isLoadedAsPlugIn);
        }


        protected virtual void SetDependencies(List<CafModuleDescriptor> modules, CafModuleDescriptor module)
        {
            foreach (var dependedModuleType in CafModuleHelper.GetDependedModuleTypes(module.Type))
            {
                var dependedModule = modules.FirstOrDefault(m => m.Type == dependedModuleType);
                if (dependedModule == null)
                {
                    throw new CafException("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + module.Type.AssemblyQualifiedName);
                }

                module.AddDependency(dependedModule);
            }
        }
        protected virtual void ConfigureServices(List<ICafModuleDescriptor> modules, IServiceCollection services, IConfiguration configuration)
        {
            var context = new CafConfigurationContext(services, configuration);
            services.AddSingleton(context);
            services.AddAssembly(typeof(CafModule).Assembly);
            foreach (var module in modules)
            {
                if (module.Instance is CafModule cafModule)
                {
                    cafModule.ServiceConfigurationContext = context;
                }
            }

            //BeforeConfigureServices
            foreach (var module in modules.Where(m => m.Instance is ICafModule))
            {
                (module.Instance).BeforeConfigureServices(context);
            }

            //ConfigureServices
            foreach (var module in modules)
            {
                if (module.Instance is CafModule cafModule)
                {
                    if (!cafModule.SkipAutoServiceRegistration)
                    {
                        services.AddAssembly(module.Type.Assembly);
                    }
                }

                module.Instance.ConfigureServices(context);
            }

            //AfterConfigureServices
            foreach (var module in modules.Where(m => m.Instance is ICafModule))
            {
                (module.Instance).AfterConfigureServices(context);
            }

            foreach (var module in modules)
            {
                if (module.Instance is CafModule cafModule)
                {
                    cafModule.ServiceConfigurationContext = null;
                }
            }
        }

        private List<T> SortByDependencies<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                SortByDependenciesVisit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        private void SortByDependenciesVisit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found! Item: " + item);
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        SortByDependenciesVisit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}
