using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caf.Core.DependencyInjection;
using Caf.Core.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cafgemini.Caf
{
    public static class CafApplicationContextExtensions
    {
        public static IWebHostEnvironment GetEnvironment(this CafApplicationContext context)
        {
            return context.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        }
        public static IApplicationBuilder GetApplicationBuilder(this CafApplicationContext context)
        {
            return context.ServiceProvider.GetRequiredService<ObjectWrapper<IApplicationBuilder>>().Value;
        }
    }
}
