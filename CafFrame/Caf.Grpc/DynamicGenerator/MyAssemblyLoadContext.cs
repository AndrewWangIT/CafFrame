using System.Reflection;
using System.Runtime.Loader;

namespace Cafgemini.Frame.Grpc.Server.DynamicGenerator
{
    public class MyAssemblyLoadContext : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}