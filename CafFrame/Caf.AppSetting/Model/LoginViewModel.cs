using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.AppSetting.Model
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class LoginResViewModel 
    {
        public string Token { get; set; }
    }
}
