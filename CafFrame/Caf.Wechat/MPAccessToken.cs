using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Wechat
{
    [Serializable]
    public class MPAccessTokenInfo
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 凭证过期时间
        /// </summary>
        public long expires_in { get; set; }
    }
}
