using Caf.Core.DataModel.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.AppSetting
{
    public class AppSettingPageRequest : PagedRepuestInput
    {
        public string Key { get; set; }
    }
}
