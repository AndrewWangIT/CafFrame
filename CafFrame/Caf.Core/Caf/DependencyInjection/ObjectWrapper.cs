using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.DependencyInjection
{
    public class ObjectWrapper<T> : IObjectWrapper<T>
    {
        public T Value { get; set; }

        public ObjectWrapper()
        {

        }

        public ObjectWrapper(T obj)
        {
            Value = obj;
        }
    }
}
