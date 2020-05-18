using Caf.Core.DataModel.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.AppSetting.Model
{
    public class AppSettingPageRequest: PagedRepuestInput
    {
        public string Key { get; set; }
    }
}
