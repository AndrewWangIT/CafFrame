using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core
{
    public interface ICafApplicationServiceProvider
    {
        void Run(IServiceProvider serviceProvider);
    }
}
