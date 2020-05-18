using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Cache.Models
{
    /// <summary>
    /// 使用方需在appsettings.json中配置此项
    /// </summary>
    public class CacheOptions
    {
        /// <summary>
        /// false 走memorycache
        /// </summary>
        public bool UseRedis { get; set; }

        public string Servers { get; set; }

        public string Password { get; set; }
    }
}
