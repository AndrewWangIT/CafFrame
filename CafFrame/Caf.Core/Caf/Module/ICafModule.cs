using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.Module
{
    /// <summary>
    /// Module
    /// </summary>
    public interface ICafModule
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="context"></param>
        void ConfigureServices(CafConfigurationContext context);
        /// <summary>
        /// 配置前调用
        /// </summary>
        /// <param name="context"></param>
        void BeforeConfigureServices(CafConfigurationContext context);
        /// <summary>
        /// 配置后调用
        /// </summary>
        /// <param name="context"></param>
        void AfterConfigureServices(CafConfigurationContext context);
    }
}
