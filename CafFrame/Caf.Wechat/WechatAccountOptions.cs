using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Wechat
{
    public class WechatAccountOptions
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string Token { get; set; }
        public string EncodingAESKey { get; set; }
        public string WechatName { get; set; }
    }
}
