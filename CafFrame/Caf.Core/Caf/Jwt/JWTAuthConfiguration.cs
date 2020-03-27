using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Core.JWT
{
    public class CafJwtAuthConfiguration
    {
        public string SecurityKey 
        {
            set  { 
                SymmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(value));
                SigningCredentials = new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            } 
        }

        public SymmetricSecurityKey SymmetricSecurityKey { get; private set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
        /// <summary>
        /// 自动赋值
        /// </summary>
        public SigningCredentials SigningCredentials { get; private set; }

        public TimeSpan Expiration { get; set; }
    }
}
