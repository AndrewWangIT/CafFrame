using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Caf.Core.AppSetting
{
    [Serializable]
    public class AppSettingViewModel : BaseViewModel
    {
        public long Id { get; set; }

        [StringLength(100)]
        public string Key { get; set; }

        public string Value { get; set; }

        [StringLength(100)]
        public string Description { get; set; }
    }
}
