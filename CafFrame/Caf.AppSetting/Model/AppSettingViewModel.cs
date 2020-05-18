using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.AppSetting.Model
{
    [Serializable]
    public class AppSettingViewModel: BaseViewModel
    {
        public long Id { get; set; }

        [StringLength(100)]
        public string Key { get; set; }

        public string Value { get; set; }

        [StringLength(100)]
        public string Description { get; set; }
    }
}
