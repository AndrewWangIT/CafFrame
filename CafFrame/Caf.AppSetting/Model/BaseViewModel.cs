using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.AppSetting.Model
{
    public class BaseViewModel
    {
        public DateTime CreateTime { get; set; }
        public string CreateUserId { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime LatestModifiedTime { get; set; }
    }

    public class Keys 
    {
        public const string AppSettingByCafs = nameof(AppSettingByCafs);

        public const string CafCache = nameof(CafCache);
    }
}
