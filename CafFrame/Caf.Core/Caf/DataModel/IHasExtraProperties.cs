using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core
{
    public interface IHasExtraProperties
    {
        Dictionary<string, object> ExtraProperties { get; }
    }
}
