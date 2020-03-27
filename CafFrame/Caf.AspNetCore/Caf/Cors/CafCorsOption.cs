using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.AspNetCore.Caf.Cors
{
    public class CafCorsOption
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 需要跨域的Origin
        /// </summary>
        public List<string> Origins { get; set; } = new List<string>();

        /// <summary>
        /// Appsetting中的配置节点
        /// </summary>
        public string ConfigurationSection { get; set; }
    }
}
