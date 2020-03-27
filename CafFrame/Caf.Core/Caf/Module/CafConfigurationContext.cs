using Microsoft.Extensions.DependencyInjection;
using Caf.Core.Utils;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Caf.Core.Module
{
    public class CafConfigurationContext
    {
        public IConfiguration Configuration { get; }
        public IServiceCollection Services { get; }
        public IDictionary<string, object> Items { get; }

        public object this[string key]
        {
            get => Items.GetOrDefault(key);
            set => Items[key] = value;
        }

        public CafConfigurationContext(IServiceCollection services, IConfiguration configuration)
        {
            Configuration = configuration;
            Services = services;
            Items = new Dictionary<string, object>();
        }
    }
}
