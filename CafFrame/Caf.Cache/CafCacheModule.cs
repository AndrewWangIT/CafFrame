using Caf.Cache.Models;
using Caf.Core.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Cache
{
    public class CafCacheModule : CafModule
    {
        public override void ConfigureServices(CafConfigurationContext context)
        {
            AddCaching(context);
        }

        public void AddCaching(CafConfigurationContext context)
        {
            var sec = context.Configuration.GetSection(nameof(CacheOptions));
            context.Services.Configure<CacheOptions>(sec);

            var opt = sec.Get<CacheOptions>();

            if (opt.UseRedis)
            {
                context.Services.AddSingleton<ICafCache, CafRedisCache>();
            }
            else
            {
                context.Services.AddMemoryCache();
                context.Services.AddDistributedMemoryCache()
                    .AddSingleton<ICafCache, CafMemoryCache>();
            }
        }
    }
}
