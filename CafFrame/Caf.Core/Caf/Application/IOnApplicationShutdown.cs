using Caf.Core.Module;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Caf.Core
{
    public interface IOnApplicationShutdown
    {
        /// <summary>
        /// 程序关闭时执行
        /// </summary>
        /// <param name="context"></param>
        void OnApplicationShutdown(CafApplicationContext context);
    }
}
