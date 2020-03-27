using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Caf.Core.Module
{
    /// <summary>
    /// 此接口定义了Module 的生命周期钩子函数
    /// </summary>
    public abstract class CafModule : ICafModule, IApplicationInitialization, IOnApplicationShutdown
    {
        /// <summary>
        /// 是否跳过自动注册（自动注册包含ITransientDependency...）
        /// </summary>
        protected internal bool SkipAutoServiceRegistration { get; protected set; }

        protected internal CafConfigurationContext ServiceConfigurationContext
        {
            get
            {
                if (_serviceConfigurationContext == null)
                {
                    throw new CafException($"{nameof(ServiceConfigurationContext)} is only available in the {nameof(ConfigureServices)}, {nameof(BeforeConfigureServices)} and {nameof(AfterConfigureServices)} methods.");
                }

                return _serviceConfigurationContext;
            }
            internal set => _serviceConfigurationContext = value;
        }

        private CafConfigurationContext _serviceConfigurationContext;
        public virtual void ConfigureServices(CafConfigurationContext context)
        {
            
        }

        public virtual void OnApplicationInitialization(CafApplicationContext context)
        {
            
        }

        public virtual void OnApplicationShutdown(CafApplicationContext context)
        {
            
        }

        public virtual void OnPostApplicationInitialization(CafApplicationContext context)
        {
            
        }

        public virtual void OnPreApplicationInitialization(CafApplicationContext context)
        {
            
        }

        public virtual void AfterConfigureServices(CafConfigurationContext context)
        {
            
        }

        public virtual void BeforeConfigureServices(CafConfigurationContext context)
        {
            
        }
        public static bool IsCafModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return
                typeInfo.IsClass &&
                !typeInfo.IsAbstract &&
                !typeInfo.IsGenericType &&
                typeof(ICafModule).GetTypeInfo().IsAssignableFrom(type);
        }

        internal static void CheckCafModuleType(Type moduleType)
        {
            if (!IsCafModule(moduleType))
            {
                throw new ArgumentException("Given type is not an Caf module: " + moduleType.AssemblyQualifiedName);
            }
        }
        protected void Configure<TOptions>(Action<TOptions> configureOptions)
    where TOptions : class
        {
            ServiceConfigurationContext.Services.Configure(configureOptions);
        }

        protected void Configure<TOptions>(string name, Action<TOptions> configureOptions)
            where TOptions : class
        {
            ServiceConfigurationContext.Services.Configure(name, configureOptions);
        }

        protected void Configure<TOptions>(IConfiguration configuration)
            where TOptions : class
        {
            ServiceConfigurationContext.Services.Configure<TOptions>(configuration);
        }

        protected void Configure<TOptions>(IConfiguration configuration, Action<BinderOptions> configureBinder)
            where TOptions : class
        {
            ServiceConfigurationContext.Services.Configure<TOptions>(configuration, configureBinder);
        }

        protected void Configure<TOptions>(string name, IConfiguration configuration)
            where TOptions : class
        {
            ServiceConfigurationContext.Services.Configure<TOptions>(name, configuration);
        }
    }
}
