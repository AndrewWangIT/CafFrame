using Caf.Core.Module;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Caf.Core.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Caf.Cache;
using Microsoft.Extensions.Options;

namespace Caf.Wechat
{
  public  class CafWeChatModule: CafModule
    {

        public override void BeforeConfigureServices(CafConfigurationContext context)
        {
            //context.Services.AddObjectWrapper(new CafCorsOption());
        }
        public override void ConfigureServices(CafConfigurationContext context)
        {
            AddWexinService(context);
            
        }
        public override void OnApplicationInitialization(CafApplicationContext context)
        {
            var app = context.ServiceProvider.GetRequiredService<ObjectWrapper<IApplicationBuilder>>().Value; ;
            var _cacheOpetions = context.ServiceProvider.GetService<IOptions<TakedaCacheOptions>>();
            var wechatAccoutnOpt = context.ServiceProvider.GetService<IOptions<WechatAccountOptions>>();
            if (_cacheOpetions.Value.UseRedis)
            {
                app.UseSenparcWeiXinWithRedis(_cacheOpetions, wechatAccoutnOpt);
            }
            else
            {
                app.UseSenparcWeiXin(_cacheOpetions, wechatAccoutnOpt);
            }
        }
        private static void AddWexinService(CafConfigurationContext context)
        {
           context.Services.AddSenparcWeixin(context.Configuration);
        }
      
    }
}
