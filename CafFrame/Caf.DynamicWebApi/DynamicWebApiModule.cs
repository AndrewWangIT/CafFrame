using System;
using Caf.Core.Module;
namespace Caf.DynamicWebApi
{
    /// <summary>
    /// 动态代理webpai
    /// </summary>
    public class DynamicWebApiModule:CafModule
    {
        // public override void ConfigureServices(CafConfigurationContext context)
        // {
        //     context.Services.AddControllers().AddDynamicWebApi();
        // }   

         public override void AfterConfigureServices(CafConfigurationContext context)
         {
            // context.Services.AddDynamicWebApi();
         }
    }
}
