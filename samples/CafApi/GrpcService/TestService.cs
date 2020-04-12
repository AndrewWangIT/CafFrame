using MagicOnion;
using MagicOnion.Server;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace CafApi.GrpcService
{
    public class TestService : ServiceBase<ITestService>, ITestService
    {
        private readonly IServiceScopeFactory _service;
        public TestService(IServiceScopeFactory service)
        { 
            _service = service; 
        }

        public async UnaryResult<string> GetTestData()
        {
            var a = await Task.FromResult("Task");
            //var a = _service.SayHello("s");
            return a;
        }
    }
}
