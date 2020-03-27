using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.Caf.User
{
    public interface ICurrentUser
    {
        long UserId { get; set; }
    }
}
