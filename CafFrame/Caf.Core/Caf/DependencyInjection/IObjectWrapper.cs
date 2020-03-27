using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.DependencyInjection
{
    public interface IObjectWrapper<T>
    {
        T Value { get; set; }
    }
}
