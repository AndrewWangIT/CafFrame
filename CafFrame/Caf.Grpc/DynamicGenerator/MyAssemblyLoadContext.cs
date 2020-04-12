using System.Reflection;
using System.Runtime.Loader;

namespace Capgemini.Frame.Grpc.Server.DynamicGenerator
{
    public class MyAssemblyLoadContext : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}