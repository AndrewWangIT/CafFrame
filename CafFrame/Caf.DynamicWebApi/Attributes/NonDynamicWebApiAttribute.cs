using System;
using System.Reflection;
namespace Caf.DynamicWebApi.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class NonDynamicWebApiAttribute:Attribute
    {
        
    }
}