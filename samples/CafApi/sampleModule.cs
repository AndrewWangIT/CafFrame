using Caf.Core.Module;
using Caf.DynamicWebApi;
using Capgemini.Frame.AspNetCore;

namespace CafApi
{
    [UsingModule(typeof(CafAspNetCoreModule))]
    [UsingModule(typeof(DynamicWebApiModule))]
    public class sampleModule:CafModule
    {
        
    }
}