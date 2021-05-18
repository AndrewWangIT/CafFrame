using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.AppSetting
{
    public class AccountOption
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }


    public class ConnectionStrings 
    {
        public string AppSettingsConnection { get; set; }
    }
}
