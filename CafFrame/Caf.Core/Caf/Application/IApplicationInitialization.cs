using Caf.Core.Module;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Caf.Core
{
    /// <summary>
    /// Application 的生命周期钩子函数
    /// </summary>
    public interface IApplicationInitialization
    {
        /// <summary>
        /// 启动前执行
        /// </summary>
        /// <param name="context"></param>
        void OnPreApplicationInitialization(CafApplicationContext context);
        /// <summary>
        /// 启动时执行
        /// </summary>
        /// <param name="context"></param>
        void OnApplicationInitialization(CafApplicationContext context);
        /// <summary>
        /// 启动后执行
        /// </summary>
        /// <param name="context"></param>
        void OnPostApplicationInitialization(CafApplicationContext context);
    }
}
