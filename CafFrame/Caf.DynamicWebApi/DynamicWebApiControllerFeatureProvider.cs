
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
namespace Caf.DynamicWebApi
{
    public class DynamicWebApiControllerFeatureProvider : ControllerFeatureProvider
    {
       

        protected override bool IsController(TypeInfo typeInfo)
        {
            if (typeof(IDynamicWebApiService).IsAssignableFrom(typeInfo))
            {
                if(!typeInfo.IsAbstract &&
                   !typeInfo.IsInterface
                   && !typeInfo.IsGenericType
                   && typeInfo.IsPublic)
                   {
                       return true;
                   }
            }
            return false;
        }
    }
}