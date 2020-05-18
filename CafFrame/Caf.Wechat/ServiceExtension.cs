using Caf.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin.Entities;
using Senparc.Weixin.RegisterServices;
using Senparc.Weixin.Cache.Redis;
using System;
using System.Collections.Generic;
using Senparc.Weixin.MP.Containers;
using Caf.Cache.Models;

namespace Caf.Wechat
{
    public static class ServiceExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddSenparcWeixin(this IServiceCollection services, IConfiguration config)
        {

            services.AddSenparcGlobalServices(config)//Senparc.CO2NET 全局注册
                .AddSenparcWeixinServices(config);//Senparc.Weixin 注册
            SenparcDI.GlobalServiceCollection = services;

            var opCacheOptionsSection = config.GetSection(nameof(CacheOptions));
            services.Configure<CacheOptions>(opCacheOptionsSection);
            services.Configure<CacheOptions>(config.GetSection("TakedaCacheOptions"));
            services.Configure<WechatAccountOptions>(config.GetSection("WechatAccountOptions"));
            services.AddTransient<IWeiXinService,WeiXinService>();

            var opCacheOptions = opCacheOptionsSection.Get<CacheOptions>();
            if (opCacheOptions.UseRedis)
            {
                var cache_Redis_Configuration = opCacheOptionsSection.Get<CacheOptions>().Servers;
                services.Configure<SenparcSetting>(senparcSetting =>
                {
                    senparcSetting.DefaultCacheNamespace = "TakedaWechatOP";
                    senparcSetting.Cache_Redis_Configuration = cache_Redis_Configuration;
                    senparcSetting.IsDebug = false;
                });
            }
            return services;
        }


        /// <summary>
        /// 微信信息缓存到本地
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <returns></returns>        
        public static IApplicationBuilder UseSenparcWeiXin(this IApplicationBuilder app, IOptions<CacheOptions> options, IOptions<WechatAccountOptions> wechatAccoutnOpt)
        {
            var cacheStrategyList = new List<Senparc.CO2NET.Cache.IDomainExtensionCacheStrategy>() {
                Senparc.Weixin.Cache.LocalContainerCacheStrategy.Instance
            };
            var senparcsetting = new SenparcSetting
            {
                Cache_Redis_Configuration = options.Value.Servers,
                IsDebug = false
            };
            AccessTokenContainer.Register(wechatAccoutnOpt.Value.AppId, wechatAccoutnOpt.Value.AppSecret, wechatAccoutnOpt.Value.WechatName);
            var register = Senparc.CO2NET.RegisterServices.RegisterService.Start(senparcsetting).UseSenparcGlobal(false, () => cacheStrategyList);            
            return app;
        }

        /// <summary>
        /// 微信信息缓存到Redis
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSenparcWeiXinWithRedis(this IApplicationBuilder app, IOptions<CacheOptions> options, IOptions<WechatAccountOptions> wechatAccoutnOpt)
        {
            var senparcsetting = new SenparcSetting
            {
                Cache_Redis_Configuration = options.Value.Servers,
                DefaultCacheNamespace = "TakedaWechatOP",
                IsDebug = false
            };

            AccessTokenContainer.Register(wechatAccoutnOpt.Value.AppId, wechatAccoutnOpt.Value.AppSecret, wechatAccoutnOpt.Value.WechatName);

            var register = Senparc.CO2NET.RegisterServices.RegisterService.Start(senparcsetting).UseSenparcGlobal();

            Senparc.CO2NET.Cache.Redis.Register.SetConfigurationOption(options.Value.Servers);
            Senparc.CO2NET.Cache.Redis.Register.UseKeyValueRedisNow();
            app.UseSenparcWeixinCacheRedis();
            var senparcWeixinSetting = new SenparcWeixinSetting
            {
                IsDebug = false,
                Items = new SenparcWeixinSettingItemCollection()
            };

            Senparc.Weixin.Config.SenparcWeixinSetting = senparcWeixinSetting;

            Senparc.NeuChar.Register.RegisterApiBind(true);
            return app;
        }
    }
}
