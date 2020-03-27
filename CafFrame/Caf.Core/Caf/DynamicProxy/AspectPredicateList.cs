using AspectCore.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core.DynamicProxy
{
    public class InterceptorContextList : List<InterceptorContext>
    {
    }

    public class InterceptorContext
    {
        public Type TInterceptor { get; set; }
        public AspectPredicate[] AspectPredicates { get; set; }
    }
}
