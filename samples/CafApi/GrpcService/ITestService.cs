using MagicOnion;
using MagicOnion.Server;
using System.Threading.Tasks;

namespace CafApi
{
    public interface ITestService : IService<ITestService>
    {
        /// <summary>
        /// grpc服务
        /// </summary>
        /// <returns></returns>
        UnaryResult<string> GetTestData();
    }
}
